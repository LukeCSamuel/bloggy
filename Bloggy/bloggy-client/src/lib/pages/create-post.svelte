<script lang="ts">
  import Edit from "../components/edit.svelte";
  import { createPostAsync, uploadImageAsync } from "../entities/post";
  import Plus from "../icons/plus.svelte";
  import Xmark from "../icons/xmark.svelte";
  import { getRouter } from "../router/configure-routes.svelte";

  const router = getRouter();
  router.setMeta({
    tabName,
  });

  interface ImageData {
    name: string;
    blob: Blob;
    src: string;
  }

  let title = $state("");
  let text = $state("");
  let imageFiles = $state(new DataTransfer().files);
  let images = $state([]) as ImageData[];

  let posting = $state(false);
  let canPost = $derived(
    !posting &&
      !!title?.trim() &&
      !!text?.trim() &&
      images.length == imageFiles.length,
  );

  // Automatically update blobs and data urls from files
  $effect(() => void processImageFiles(imageFiles));

  async function processImageFiles(files: FileList) {
    await Promise.resolve();
    const imageList = [...images];
    for (const file of files) {
      const arrayBuffer = await file.arrayBuffer();
      const blob = new Blob([new Uint8Array(arrayBuffer)], {
        type: file.type,
      });
      const src = URL.createObjectURL(blob);

      // if an image with that name exists, remove it and replace it
      const existing = imageList.findIndex((img) => img.name === file.name);
      if (existing !== -1) {
        imageList.splice(existing, 1);
      }

      imageList.push({
        name: file.name,
        blob,
        src,
      });
    }
    images = imageList;
  }

  function removeImage(name: string) {
    const index = images.findIndex((img) => img.name === name);
    if (index !== -1) {
      images.splice(index, 1);
    }
  }

  async function createPost() {
    if (!canPost) {
      return;
    }

    posting = true;
    const { post } =
      (await createPostAsync({
        title,
        text,
      })) ?? {};

    if (post) {
      for (const { blob } of images) {
        await uploadImageAsync(post.id, blob);
      }

      // Navigate to home page after posting
      router.navigateTo({ name: "bloggy/home" });
    }
  }
</script>

{#snippet tabName()}
  <Plus /> New Post
{/snippet}

<div class="p-4">
  <!-- Title -->
  <Edit
    class="mb-2 font-slab text-xl"
    placeholder="Add title..."
    bind:value={title}
  />
  <!-- Images -->
  <div class="my-4">
    <div class="grid grid-cols-3 gap-4 mb-4">
      {#each images as { name, src }}
        <div
          class="relative border-4 border-solid border-gray-100 drop-shadow-sm"
        >
          <img {src} alt="unknown" />
          <button
            class="absolute -top-2 -right-2 w-6 h-6 flex items-center justify-center bg-red-400 text-xl text-white rounded-full"
            onclick={() => removeImage(name)}
          >
            <Xmark />
          </button>
        </div>
      {/each}
    </div>
    <label class="grid">
      <input
        class="row-start-1 col-start-1 opacity-0"
        type="file"
        accept="image/png, image/jpeg"
        multiple
        bind:files={imageFiles}
      />
      <span
        class="row-start-1 col-start-1 c-btn w-max px-3 py-0.5 bg-yellow-300"
      >
        <Plus /> Add images
      </span>
    </label>
  </div>
  <!-- Text -->
  <Edit class="mb-2" placeholder="Add text..." bind:value={text} />
  <div class="flex justify-end">
    <button
      class="c-btn px-3 py-0.5 bg-yellow-300 z-10"
      disabled={!canPost}
      onclick={createPost}
    >
      Post
    </button>
  </div>
</div>
