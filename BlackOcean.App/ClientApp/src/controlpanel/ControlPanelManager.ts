import {Band, ControlPanel, Scale, Status} from "./ControlPanel"

export type SubscriptionCallback = (key: string, value: unknown) => void

export class ControlPanelManager {
    private subscriptions: Map<string, SubscriptionCallback[]> = new Map()
    private controlPanel: ControlPanel | undefined
    sendMessage: ((data: unknown) => void) | undefined

    getControlPanel(): ControlPanel | undefined {
        return this.controlPanel
    }

    /**
     * Subscribe to updates on a specific key.
     */
    subscribe(key: string, callback: SubscriptionCallback): () => void {
        if (!this.subscriptions.has(key)) {
            this.subscriptions.set(key, [])
        }
        this.subscriptions.get(key)!.push(callback)

        // Return an unsubscribe function
        return () => this.unsubscribe(key, callback)
    }

    /**
     * Unsubscribe from updates on a specific key.
     */
    unsubscribe(key: string, callback: SubscriptionCallback): void {
        if (this.subscriptions.has(key)) {
            const callbacks = this.subscriptions.get(key)!
            const index = callbacks.indexOf(callback)
            if (index !== -1) {
                callbacks.splice(index, 1)
                if (callbacks.length === 0) {
                    this.subscriptions.delete(key)
                }
            }
        }
    }

    /**
     * Notify subscribers of a specific key and all parent keys.
     */
    private notifySubscribers(keys: string[], updatedValue: unknown): void {
        const notified = new Set<string>()

        for (let i = 1; i <= keys.length; i++) {
            const currentKey = keys.slice(0, i).join('.')
            if (notified.has(currentKey)) continue
            notified.add(currentKey)

            if (this.subscriptions.has(currentKey)) {
                this.subscriptions.get(currentKey)!.forEach(callback => {
                    const value = this.getValue(currentKey)
                    try {
                        callback(currentKey, value)
                    }
                    catch (error) {
                        console.error(`Property ${currentKey} subscription failed.`, error, updatedValue)
                    }
                })
            }
        }
    }

    /**
     * Update control panel and notify subscribers.
     */
    updateControlPanel(updates: Record<string, unknown>): void {
        if (this.controlPanel === undefined) return

        for (const [key, value] of Object.entries(updates)) {
            this.setValue(key, value)
        }
    }

    sendAction(path: string, value: unknown): void {
        if (!this.sendMessage) return
        path = toPascalCase(path).join('.')
        this.sendMessage({ [path]: value })
    }

    setValue(path: string, value: unknown) {
        const keys = toCamelCase(path)

        if (this.controlPanel === undefined) return
        let target: Record<string, unknown> | undefined = this.controlPanel

        // Traverse through the keys except for the last one
        for (let i = 0; i < keys.length - 1; i++) {
            if (!(keys[i] in target)) {
                console.warn(`Key "${keys[i]}" does not exist in "${path}".`)
                return
            }
            target = target[keys[i]] as Record<string, unknown>
        }

        // Update the final key
        const finalKey = keys[keys.length - 1]
        if (!(finalKey in target)) {
            console.warn(`Key "${finalKey}" does not exist in "${path}".`)
            return
        }

        target[finalKey] = value

        // Notify subscribers
        this.notifySubscribers(keys, value)
    }

    /**
     * Get the value of a property by path.
     */
    getValue(path: string): unknown {
        if (this.controlPanel === undefined) return undefined

        const keys = toCamelCase(path)
        let target: unknown = this.controlPanel

        for (const key of keys) {
            if (target && typeof target === 'object' && key in target) {
                target = (target as Record<string, unknown>)[key]
            } else {
                console.warn(`Key "${key}" does not exist in the target object.`)
                return undefined
            }
        }

        return target
    }

    loadControlPanel(controlPanel: ControlPanel): void {
        // Replace the control panel
        this.controlPanel = controlPanel

        console.info(controlPanel)

        // Notify all subscribers that it has been changed
        for (const [key, subscribers] of this.subscriptions) {
            const value = this.getValue(key)
            for (const callback of subscribers) {
                try {
                    callback(key, value)
                }
                catch (error) {
                    console.error(`Property ${key} subscription failed.`, error, value)
                }
            }
        }
    }
}

export const controlPanelManager = new ControlPanelManager()

const bandMap: Record<Status, string> = {
    [Status.None]: "none",
    [Status.Safe]: "safe",
    [Status.Warn]: "warn",
    [Status.Danger]: "danger",
}

export function translateBands(bands: Band[] | undefined): string {
    if (bands === undefined || bands.length == 0) return bandMap[Status.None]
    return bandMap[bands[0].status] + ' '
        + bands.slice(1).map(b => `${b.value} ${bandMap[b.status]}`).join(' ')
}

export function translateScale(scale: Scale): string {
    return scale.toString().toLowerCase()
}

export function toPascalCase(path: string): string[] {
    return path
        .split('.')
        .map(segment => segment.charAt(0).toUpperCase() + segment.slice(1))
}

export function toCamelCase(path: string): string[] {
    return path
        .split('.')
        .map(segment => segment.charAt(0).toLowerCase() + segment.slice(1))
}