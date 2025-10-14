# bloggy
Halloween 2025 activity

## Capture image from camera

```js
// Should use an existing video element in DOM so user can preview the camera
const video = document.createElement('video');
// Could probably just reuse existing video element actually?
const photo = document.createElement('img');

async function startRecording () {
  const stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: false });
  video.srcObject = stream;
  video.play();
}

async function takePicture () {
  const canvas = new OffscreenCanvas(video.videoWidth, video.videoHeight);
  const gc = canvas.getContext('2d');
  gc.drawImage(video, 0, 0);
  const blob = await canvas.convertToBlob();
  // preview image
  const fileReader = new FileReader();
  fileReader.addEventListener('load', (e) => { photo.setAttribute('src', e.target.result) });
  fileReader.readAsDataURL(blob);
  // or upload blob to server!
}
```
