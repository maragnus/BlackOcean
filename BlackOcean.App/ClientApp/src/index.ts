import './aries/index'
import { WebSocketClientService } from './controlpanel/WebSocketService'

import './styles/styles.scss'

console.log("Starting ARIES...")

const href = (window.location.port === '44414') 
    ? `ws://${window.location.hostname}:5226/ws`
    : `ws://${window.location.host}/ws`

const client = new WebSocketClientService(href)
client.connect();

document.addEventListener("contextmenu", function(event) { event.preventDefault(); })