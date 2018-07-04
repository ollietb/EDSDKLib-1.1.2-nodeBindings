const path = require('path');
const shell = require('shelljs');
const fse = require('fs-extra');

const buildDir = path.join(__dirname, '../NodeBindings/bin/Debug');
const externalDir = path.join(__dirname, '../NodeBindings/external');
const prebuiltDir = path.join(__dirname, '../node-lib/prebuilt');

function clearBuildDir() {
    console.log(`Clearing build dir "${buildDir}"`);
    Promise.resolve()
        .then(() => fse.remove(buildDir));
}

/**
 * @return {Promise}
 */
function rebuildSolution() {
    const command = `msbuild EDSDKLibNodeBindings.sln /t:rebuild`;
    console.log(`Rebuilding solution with command: ${command}`);
    return new Promise((resolve, reject) => {
        shell.exec(command, (code, stdout, stderr) => {
            console.log(`Program output:\n${stdout || 'None'}\n`);
            console.log(`Program stderr:\n${stderr || 'None'}\n`);
            if (code !== 0) {
                console.log(`Exit code: ${code}`);
                reject(new Error(`Failed solution rebuild, exit code: ${code}`));
            }

           resolve();
        });
    });
}

/**
 * @return {Promise}
 */
function copyExternalToBuildDir() {
    console.log(`Copy files from "${externalDir}" to "${buildDir}"`);

    const fileNames = [
        'DPP4Lib',
        'IHL',
        'EDSDK.dll',
        'EdsImage.dll',
        'System.IO.Compression.dll',
    ];

    return Promise.all(fileNames.map(fileName =>
        fse.copy(
            path.join(externalDir, fileName),
            path.join(buildDir, fileName)
        )
    ));
}

/**
 * @return {Promise}
 */
function copyToPrebuiltDir() {
    console.log(`Copy files from "${buildDir}" to "${prebuiltDir}"`);

    return Promise.resolve()
        .then(() => fse.remove(prebuiltDir))
        .then(() => fse.copy(buildDir, prebuiltDir));
}

Promise.resolve()
    .then(() => clearBuildDir())
    .then(() => rebuildSolution())
    .then(() => copyExternalToBuildDir())
    .then(() => copyToPrebuiltDir())
    .catch(e => {
        console.log(`Error: ${e.stack}`);
        process.exit(1);
    });

