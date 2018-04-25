const fs = require('fs');
const fileType = require('file-type');
const path = require('path');
const app = require('http').createServer(handler);
const {
    setOutputPath,
    takePhoto,
    beginSession,
    endSession,
    startLiveView,
    stopLiveView,
    startVideo,
    stopVideo,
    getLastDownloadedImageFilename,
    getPreviewImage,
    callCameraMethod,
} = require('../node_bindings/index');
const io = require('socket.io')(app);
const port = 9003;

console.log(`Server starting at http://localhost:${port}`);

app.listen(port);

function handler(req, res) {
    const url = req.url;

    if (url === '/') {
        res.writeHead(200, { 'Content-Type': 'text/plain' });
        res.end('Server running');
    } else {
        fs.readFile(path.join(__dirname, url), (error, data) => {
            if (error) {
                res.writeHead(404);
                res.end(`Invalid url "${url}"`);
            } else {
                const fileInfo = fileType(data);

                if (fileInfo) {
                    res.writeHead(200, { 'Content-Type': fileInfo.mime });
                } else {
                    res.writeHead(200);
                }
                res.end(data);
            }
        });
    }
}

let sessionStarted = false;

const frameRate = 20; // fps
let previewLoopHandler = null;
let previewRunning = false;
const savePath = path.join(__dirname, 'tmp');

let clientSocket = null;


io.on('connection', function(socket) {
    console.log('Client connected');
    clientSocket = socket;

    clientSocket.on('startPreview', () => {
        console.log('Starting preview');
        startPreview();
    });

    clientSocket.on('stopPreview', () => {
        console.log('Stopping preview');
        stopPreview();
    });

    clientSocket.on('capturePhoto', (data, callback) => {
        console.log('Capturing photo');
        capturePhoto(callback);
    });

    clientSocket.on('startVideo', () => {
        console.log('Starting video');
        startCaptureVideo();
    });

    clientSocket.on('stopVideo', (data, callback) => {
        console.log('Stopping video');
        stopCaptureVideo(callback);
    });

    clientSocket.on('initCamera', () => {
        console.log('Initialising camera');

        if (sessionStarted) {
            console.log('Camera session already started');
        } else {
            console.log('Starting camera session');

            Promise.resolve()
                .then(() => beginSession())
                .then(() => setOutputPath({ outputPath: savePath }))
                .then(() => console.log('Camera session started'))
                .catch(handleError);
        }
    });
});

function startPreview() {
    previewRunning = true;
    startLiveView()
        .then(() => previewLoop())
        .catch(handleError);
}

function stopPreview() {
    previewRunning = false;
    clearTimeout(previewLoopHandler);
    stopLiveView()
        .catch(handleError);
}

function capturePhoto(callback) {
    takePhoto()
        .then((result) => fs.readFileSync(result.path))
        .then(buffer => {
            callback('data:image/jpeg;base64,' + buffer.toString('base64'));
        })
        .catch(handleError);
}

function startCaptureVideo() {
    startVideo()
        .catch(handleError);
}

function stopCaptureVideo(callback) {
    stopVideo()
        .then((result) => callback(path.relative(savePath, result.path)))
        .catch(handleError);
}

function previewLoop() {
    if (!previewRunning) {
        return;
    }

    getPreviewImage()
        .then(result => {
            const data = 'data:image/jpeg;base64,' + result.bitmap.toString('base64');
            clientSocket.emit('preview', data);

            previewLoopHandler = setTimeout(() => {
                previewLoop();
            }, 1000 / frameRate);
        })
        .catch(e => {
            console.log('Preview loop error: ' + e.message);

            previewLoopHandler = setTimeout(() => {
                previewLoop();
            }, 1000 / frameRate);
        });
}

function handleError(e) {
    console.log(`Error: ${e.message}`, e);
}
