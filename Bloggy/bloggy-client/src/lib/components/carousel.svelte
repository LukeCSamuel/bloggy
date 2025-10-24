<script lang="ts">
  import ChevronLeftCircle from "../icons/chevron-left-circle.svelte";

  interface Props {
    images: string[];
  }

  let { images }: Props = $props();
  let currentIndex = $state(0);
  let container: HTMLElement = $state()!;

  // Monitor the scroll container for changes in viewability to update the index
  $effect(() => {
    if (!container || container.children?.length < 1) {
      return;
    }

    const observer = new IntersectionObserver(
      (intersections) => {
        for (const intersection of intersections) {
          if (intersection.intersectionRatio > 0.5) {
            if (intersection.target instanceof HTMLImageElement) {
              const index = images.findIndex(
                (image) => image === intersection.target.getAttribute("src"),
              );
              currentIndex = index;
            }
          }
        }
      },
      {
        threshold: 0.5,
      },
    );

    // Wait for loading to finish before setting index
    for (const child of container.children) {
      observer.observe(child);
    }
    setTimeout(() => {
      currentIndex = 0;
    }, 0);

    return () => {
      observer.disconnect();
    };
  });

  // Monitor manual index change
  $effect(() => {
    if (!container || container.children?.length < 1) {
      return;
    }

    // Find the element for the specified index
    const el = [...container.children].at(currentIndex);
    if (!el) {
      return;
    }

    const left = el.getBoundingClientRect().left + container.scrollLeft;
    container.scrollTo({
      left,
      behavior: "smooth",
    });
  });

  function previous() {
    currentIndex--;
  }
  function next() {
    currentIndex++;
  }
</script>

{#if images.length > 0}
  <div class="relative text-white text-3xl">
    <div
      class="c-img-grid items-center bg-black overflow-x-scroll snap-x snap-mandatory"
      bind:this={container}
    >
      {#each images as image}
        <img class="snap-center" src={image} alt="unknown" />
      {/each}
    </div>
    {#if currentIndex > 0}
      <button
        class="absolute left-2 top-[50%] -translate-y-[50%]"
        onclick={previous}
        aria-label="previous"
      >
        <ChevronLeftCircle />
      </button>
    {/if}
    {#if currentIndex < images.length - 1}
      <button
        class="absolute right-2 top-[50%] -translate-y-[50%] rotate-180"
        onclick={next}
        aria-label="next"
      >
        <ChevronLeftCircle />
      </button>
    {/if}
  </div>
{/if}

<style lang="css">
  @reference "../../app.css";

  .c-img-grid {
    grid-auto-columns: 100%;
    grid-auto-flow: column;
    @apply grid;
  }
</style>
