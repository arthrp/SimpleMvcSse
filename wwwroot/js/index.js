console.log('Hello');

let sseEventSource = null;

function registerError(source) {
    source.onerror = (e) => {
        console.error(e);
        source.close();
        sseEventSource = null;
    }
}

function registerMessage(source) {
    source.onmessage = (m) => {
        console.log(m.data);
        document.querySelector("#events-container ul").innerHTML += `<li>${m.data}</li>`
    }
}

document.querySelector("#start-infinite").addEventListener('click', (e) => {
    if(sseEventSource !== null) return;
    
    console.log('starting');
    sseEventSource = new EventSource('value/infinitestream');
    registerError(sseEventSource);
    registerMessage(sseEventSource);
});

document.querySelector('#stop-all').addEventListener('click', () => {
    if(sseEventSource !== null){
        console.log('stopping');
        sseEventSource.close();
        sseEventSource = null;
    }
});

document.querySelector('#start-feedback').addEventListener('click', () => {
    const items = $('#feedback-options').val();
    if(sseEventSource !== null || items.length === 0) return;

    console.log('starting',items);
    const ids = items.join(',');
    sseEventSource = new EventSource('value/feedback?idsStr='+ids, {withCredentials: true});
    registerError(sseEventSource);
    registerMessage(sseEventSource);
});