const preview = document.getElementById('preview');
const previewContext = preview.getContext('2d');

const photos = document.getElementById('photos');

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
    console.log('Starting preview');
    socket.emit('startPreview');
}

function stopPreview() {
    console.log('Stopping preview');
    socket.emit('stopPreview');
}

function capturePhoto() {
    console.log('Capturing photo');
    socket.emit('capturePhoto', null, data => {
        const img = document.createElement('img');
        img.src = data;
        photos.appendChild(img);
    });
}

function startVideo() {
    console.log('Starting video');
    socket.emit('startVideo');
}

function stopVideo() {
    console.log('Stopping video');
    socket.emit('stopVideo', null, data => {
        const video = document.createElement('video');
        video.loop = true;
        video.autoplay = true;
        video.src = './tmp/' + data;
        photos.appendChild(video);
    });
}
