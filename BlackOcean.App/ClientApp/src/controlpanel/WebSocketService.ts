import { controlPanelManager } from './ControlPanelManager'
import { ControlPanel } from './ControlPanel'

export class WebSocketClientService {
    private ws: WebSocket | undefined = undefined

    constructor(private url: string) {}

    /**
     * Connect to the WebSocket server.
     */
    connect(): void {
        this.ws = new WebSocket(this.url)

        this.ws.onopen = () => {
            console.log("WebSocket connection established.")
            controlPanelManager.sendMessage = this.sendMessage.bind(this)
        }

        this.ws.onmessage = (event: MessageEvent) => {
            this.handleMessage(event.data)
        }

        this.ws.onclose = () => {
            controlPanelManager.sendMessage = undefined
            console.log("WebSocket connection closed. Attempting to reconnect...")
            setTimeout(() => this.connect(), 5000)
        }

        this.ws.onerror = (error: Event) => {
            console.error("WebSocket encountered an error:", error)
        }
    }

    private sendMessage(data: unknown): void {
        const message = JSON.stringify({ ControlPanel: data })
        this.ws?.send(message)
        console.info("Sent message", message)
    }

    /**
     * Handle incoming WebSocket messages.
     */
    private handleMessage(data: string): void {
        try {
            const message = JSON.parse(data)

            if (message.controlPanel) {
                if (message.controlPanel.replace) {
                    this.handleReplace(message.controlPanel.replace)
                } else if (message.controlPanel.updates) {
                    this.handleUpdates(message.controlPanel.updates)
                } else {
                    console.warn("Unknown ControlPanel command:", message.controlPanel)
                }
            } else {
                console.warn("Unhandled message structure:", message.controlPanel)
            }
        } catch (error) {
            console.error("Failed to parse WebSocket message:", data, error)
        }
    }

    /**
     * Handle the `Replace` command to load a new ControlPanel.
     */
    private handleReplace(replace: ControlPanel): void {
        console.log("Handling Replace command.")
        controlPanelManager.loadControlPanel(replace)
    }

    /**
     * Handle the `Updates` command to update properties in the ControlPanel.
     */
    private handleUpdates(updates: Record<string, unknown>): void {
        controlPanelManager.updateControlPanel(updates)
    }
}
