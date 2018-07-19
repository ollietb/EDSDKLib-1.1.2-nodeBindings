const fs = require('fs');
const path = require('path');

const {
    setOutputPath,
    takePhoto,
    beginSession,
    endSession,
    startLiveView,
    stopLiveView,
    startVideo,
    stopVideo,
    getPreviewImage,
    callCameraMethod,
} = require('./index');

const benchmark = (message, promiseOrFunction) => {
    const ss = Date.now();

    return Promise.resolve()
        .then(() => typeof promiseOrFunction === 'function' ? promiseOrFunction() : promiseOrFunction)
        .then(result => {
            console.log(`${message}: ${Date.now() - ss}ms`);
            return result;
        });
};
const delay = (time) => new Promise(r => setTimeout(r, time));

const outputPath = 'C:\\pictures';

Promise.resolve()
    .then(() => benchmark('beginSession', () => beginSession()))
    .then(() => benchmark('setOutputPath', () => setOutputPath({ outputPath })))
    .then(() => benchmark('startLiveView', () => startLiveView()))
    .then(() => benchmark('takePhoto', () => takePhoto()))
    .then(() => benchmark('getPreviewImage', getPreviewImage()))
    .then(() => benchmark('getPreviewImage', getPreviewImage()))
    .then(result => fs.writeFile(
        path.join(outputPath, `live-preview-${Date.now()}.jpg`),
        result.bitmap, (err) => {}
    ))
    .then(() => benchmark('delay', () => delay(1500)))
    .then(() => benchmark('startVideo', () => startVideo()))
    .then(() => benchmark('delay', () => delay(2 * 1000)))
    .then(() => benchmark('stopVideo', () => stopVideo()))
    .then(() => benchmark('stopLiveView', () => stopLiveView()))
    .then(() => benchmark('endSession', () => endSession()))
;

// Promise.resolve()
//     .then(() => benchmark('beginSession', () => beginSession()))
//     .then(() => benchmark('setOutputPath', () => setOutputPath({ outputPath })))
//     .then(() => delay(100))
//     .then(() => benchmark('startLiveView', () => startLiveView()))
//     // .then(() => benchmark('callCameraMethod', () => callCameraMethod({ method: 'SC_TakePicture' })))
//     .then(() => benchmark('takePhoto', () => takePhoto()))
//     .then(() => delay(100))
//     .then(() => benchmark('takePhoto', () => takePhoto()))
//     .then(() => delay(100))
//     .then(() => benchmark('stopLiveView', () => stopLiveView()))
//     .then(() => benchmark('endSession', () => endSession()))
// ;

// Promise.resolve()
//     .then(() => benchmark('beginSession', () => beginSession()))
//     .then(() => benchmark('setOutputPath', () => setOutputPath({ outputPath })))
//     .then(() => benchmark('startLiveView', () => startLiveView()))
//     .then(() => benchmark('takePhoto', () => takePhoto()))
//     .then(() => benchmark('getPreviewImage', getPreviewImage()))
//     .then(() => benchmark('getPreviewImage', getPreviewImage()))
//     .then(result => fs.writeFile(
//         path.join(outputPath, `live-preview-${Date.now()}.jpg`),
//         result.bitmap, (err) => {}
//     ))
//     .then(() => benchmark('delay', () => delay(200)))
//     .then(() => benchmark('stopLiveView', () => stopLiveView()))
//     .then(() => benchmark('endSession', () => endSession()))
// ;
