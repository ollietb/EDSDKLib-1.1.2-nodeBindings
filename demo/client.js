const preview = document.getElementById('preview');
const previewContext = preview.getContext('2d');

const medias = document.getElementById('medias');

// init preview
previewContext.fillStyle = 'green';
previewContext.fillRect(0, 0, preview.width, preview.height);

const socket = io('http://localhost:9003');

socket.on('preview', data => {
    console.log('Preview received');
    const img = document.createElement('img');
    img.onload = () => {
        previewContext.drawImage(img, 0, 0, preview.width, preview.height);
    };
    img.onerror = () => {
        console.log('Invalid preview image');
    };
    img.src = data;
});

function startPreview() {
    previewContext.clearRect(0, 0, preview.width, preview.height);
    console.log('Starting preview');
    socket.emit('startPreview');
}

function stopPreview() {
    console.log('Stopping preview');
    socket.emit('stopPreview');
}

function capturePhoto() {
    console.log('Capturing photo');
    socket.emit('capturePhoto', null, src => {
        addImage(src);
    });
}

function startVideo() {
    console.log('Starting video');
    socket.emit('startVideo');
}

function stopVideo() {
    console.log('Stopping video');
    socket.emit('stopVideo');
}

function downloadLastFile() {
    console.log('Downloading last file');
    socket.emit('downloadLastFile', null, fileName => {
        addMedia(fileName);
    });
}

function addMedia(fileName) {
    if (/\.(jpg|jpeg|gif|bmp|png)$/i.test(fileName)) {
        addImage(fileName);
    } else {
        addVideo(fileName);
    }
}

function addImage(fileName) {
    const img = document.createElement('img');
    img.src = './tmp/' + fileName;
    medias.appendChild(img);
}

function addVideo(fileName) {
    const video = document.createElement('video');
    video.loop = true;
    video.autoplay = true;
    video.src = './tmp/' + fileName;
    medias.appendChild(video);
}
