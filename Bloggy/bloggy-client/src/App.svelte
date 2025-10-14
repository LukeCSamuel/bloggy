<script lang="ts">
  // import svelteLogo from './assets/svelte.svg'
  // import viteLogo from '/vite.svg'
  import RouterOutlet from './lib/router/router-outlet.svelte';
  import { router } from './routes';

  router.start();
</script>

<main>
  <svelte:boundary>
    <RouterOutlet />

    {#snippet failed(error, reset)}
      <p>Uh oh, an error occurred:</p>
      <div>
        <code>
          {error}  
        </code>
      </div>
      <div>
        <button onclick={reset}>oops! try again</button>
      </div>
    {/snippet}
  </svelte:boundary>
</main>

<style>
</style>
