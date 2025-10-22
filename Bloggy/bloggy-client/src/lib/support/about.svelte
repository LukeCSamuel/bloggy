<script>
  import Link from "../router/link.svelte";
</script>

<section>
  <h1 class="md:sticky top-4">What is Bloggy?</h1>
  <p>
    Bloggy is ...
    <!-- TODO: What is Bloggy -->
  </p>
</section>

<style lang="css">
  @reference "../../app.css";

  section {
    @apply max-w-[800px] p-4 mx-auto;
  }

  h1,
  h2 {
    @apply font-slab font-semibold;
  }

  h1 {
    @apply text-3xl mt-8 mb-6;
  }

  h2 {
    @apply text-xl mt-8 mb-3;
  }

  ol {
    @apply list-decimal pl-10;
  }

  ol > li::marker {
    @apply font-semibold;
  }

  hr {
    @apply mt-6;
  }

  p {
    @apply mb-2;
  }
</style>
