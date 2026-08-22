using CoreUtils.AssetBuckets;
using _Project.Code.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code.Testing
{
    public class IngredientSpawner : MonoBehaviour
    {
        // F1..F12 is the whole top row — the hard ceiling for one-key spawning.
        private const int MaxKeys = 12;

        [Tooltip("CoreUtils PrefabBucket sourcing Prefabs/IngredientsRaw.")]
        [SerializeField] private PrefabBucket bucket;

        [Tooltip("Which bucket ingredient this spawner dispenses (case-insensitive). " +
                 "Leave blank to use the GameObject name after its last underscore.")]
        [SerializeField] private string ingredientName;

        [Tooltip("Where ingredients appear. Leave empty to use this object's position.")]
        [SerializeField] private Transform spawnPoint;

        [Tooltip("Offset from the spawn point — a little height lets the ingredient " +
                 "drop in visibly instead of clipping into the floor.")]
        [SerializeField] private Vector3 spawnOffset = new(0f, 1f, 0f);

        private GameObject prefab;
        private Key spawnKey = Key.None;

        private void Start()
        {
            if (bucket == null)
            {
                Debug.LogWarning($"[IngredientSpawner] {name}: no bucket assigned.", this);
                return;
            }

            // Resolve which ingredient is ours: explicit field, or name suffix.
            var wanted = ingredientName;
            if (string.IsNullOrWhiteSpace(wanted))
            {
                var cut = name.LastIndexOf('_');
                wanted = cut >= 0 ? name[(cut + 1)..] : name;
            }

            // Sort a copy alphabetically — never trust the bucket's internal order.
            var items = (GameObject[])bucket.Items.Clone();
            System.Array.Sort(items, (a, b) =>
                string.CompareOrdinal(a != null ? a.name : "", b != null ? b.name : ""));
            for (var i = 0; i < items.Length && i < MaxKeys; i++)
            {
                if (items[i] == null ||
                    !items[i].name.Equals(wanted, System.StringComparison.OrdinalIgnoreCase)) continue;

                prefab = items[i];
                spawnKey = Key.F1 + i;
                Debug.Log($"[IngredientSpawner] {name}: dispensing {prefab.name} on {spawnKey}.");
                return;
            }

            Debug.LogWarning($"[IngredientSpawner] {name}: '{wanted}' not found in bucket '{bucket.name}'.", this);
        }

        private void Update()
        {
            // Input System only project: the legacy Input class throws rather than
            // returning false, so this test spawner would break the gym scene.
            if (prefab == null || spawnKey == Key.None) return;

            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard[spawnKey].wasPressedThisFrame)
                Spawn();
        }

        private void Spawn()
        {
            var anchor = spawnPoint != null ? spawnPoint : transform;
            var spawned = Instantiate(prefab, anchor.position + spawnOffset, Quaternion.identity);

            var label = spawned.TryGetComponent<Ingredient>(out var ingredient) && ingredient.data != null
                ? ingredient.data.ingredientName
                : prefab.name;
            Debug.Log($"[IngredientSpawner] {spawnKey} spawned: {label}");
        }
    }
}
