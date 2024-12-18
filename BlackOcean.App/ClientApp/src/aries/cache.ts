const typeCacheStore = new Map<string, Map<string, unknown>>()

export function cache<T>(typeName: string, key: object, f: () => T): T {
    const keyValue = JSON.stringify(key)

    // Get or create the cache for this specific type
    let typeCache = typeCacheStore.get(typeName)
    if (!typeCache) {
        typeCache = new Map<string, unknown>()
        typeCacheStore.set(typeName, typeCache)
    }

    // Check if the key exists in the type-specific cache
    if (typeCache.has(keyValue)) {
        return typeCache.get(keyValue) as T
    }

    // Compute the value, store it in the type-specific cache, and return it
    const value = f()
    typeCache.set(keyValue, value)
    return value
}