<script lang="ts">
  import { onDestroy } from "svelte";
  import Camera from "../icons/camera.svelte";
  import Refresh from "../icons/refresh.svelte";

  interface Props {
    blob: Blob | undefined;
  }

  let { blob = $bindable(undefined) }: Props = $props();

  let video: HTMLVideoElement;
  let previewImageUrl: string = $state("");
  let started = $state(false);
  let usingCamera = $state(false);
  let stream: MediaStream | undefined;

  async function startRecording() {
    stream = await navigator.mediaDevices.getUserMedia({
      video: true,
      audio: false,
    });
    video.srcObject = stream;
    started = true;
    usingCamera = true;
    video.play();
  }

  function tryAgain() {
    previewImageUrl = "";
    blob = undefined;
    usingCamera = true;
    video.play();
  }

  async function takePicture() {
    const canvas = new OffscreenCanvas(video.clientWidth, video.clientHeight);
    const ctx = canvas.getContext("2d");
    const diff = Math.abs(video.videoWidth - video.videoHeight);
    const min = Math.min(video.videoWidth, video.videoHeight);
    ctx?.drawImage(
      video,
      video.videoWidth > video.videoHeight ? diff / 2 : 0,
      video.videoHeight > video.videoWidth ? diff / 2 : 0,
      min,
      min,
      0,
      0,
      video.clientWidth,
      video.clientHeight,
    );
    usingCamera = false;
    video.pause();
    blob = await canvas.convertToBlob();

    const fileReader = new FileReader();
    fileReader.addEventListener("load", (e) => {
      previewImageUrl = e.target!.result as string;
    });
    fileReader.readAsDataURL(blob);
  }

  onDestroy(() => {
    stream?.getTracks().forEach(track => track.stop());
  });
</script>

<!-- svelte-ignore a11y_media_has_caption -->
<div class="grid justify-around relative">
  <div
    class="row-start-1 col-start-1 rounded-full overflow-clip w-72 h-72 z-0 inset-shadow-lg"
  >
    <video
      class="w-72 h-72 object-cover bg-black rotate-y-180"
      bind:this={video}
    ></video>
  </div>

  {#if !started}
    <div class="row-start-1 col-start-1 w-72 h-72 z-10">
      <button
        class="text-center text-gray-100 h-full w-full"
        onclick={startRecording}
      >
        <span class="text-3xl"><Camera /></span><br class="mb-[1ex]" />
        Start camera
      </button>
    </div>
  {/if}
  {#if usingCamera}
    <div
      class="c-camera-button rounded-full overflow-clip shadow-sm bg-white border-8 border-solid border-gray-200 active:bg-gray-300 active:shadow-none"
    >
      <button
        class="w-full h-full"
        aria-label="take picture"
        onclick={takePicture}
      ></button>
    </div>
  {/if}

  {#if previewImageUrl}
    <div
      class="row-start-1 col-start-1 rounded-full overflow-clip w-72 h-72 z-20 inset-shadow-lg"
    >
      <img
        class="w-72 h-72 object-cover"
        alt="your pfp preview"
        src={previewImageUrl}
      />
    </div>
    <div
      class="c-camera-button rounded-full overflow-clip z-30 shadow-sm bg-red-400 border-2 border-solid border-black active:bg-red-500 active:shadow-none"
    >
      <button
        class="w-full h-full text-white flex items-center justify-center text-5xl rotate-y-180"
        onclick={tryAgain}
      >
        <Refresh />
      </button>
    </div>
  {/if}
</div>

<style lang="css">
  @reference "../../app.css";

  .c-camera-button {
    position: absolute;
    width: --spacing(20);
    height: --spacing(20);
    bottom: 0;
    left: 50%;
    transform: translate(-50%, 50%);
  }
</style>
