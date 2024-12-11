import './aries/index'
import { WebSocketClientService } from './controlpanel/WebSocketService'

import './styles/styles.scss'

console.log("Starting ARIES...")

const client = new WebSocketClientService("http://localhost:5226/ws")
client.connect();