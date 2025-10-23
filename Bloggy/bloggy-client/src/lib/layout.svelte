<script lang="ts">
  import type { Snippet } from "svelte";
  import Footer from "./components/footer.svelte";
  import Search from "./components/search.svelte";
  import Firework from "./icons/firework.svelte";
  import Logo from "./icons/logo.svelte";
  import Notebook from "./icons/notebook.svelte";
  import Trophy from "./icons/trophy.svelte";
  import { getRouter } from "./router/configure-routes.svelte";
  import Link from "./router/link.svelte";
  import RouterOutlet from "./router/router-outlet.svelte";
  import { auth } from "./utils/auth.svelte";

  const router = getRouter();

  if (!auth.isAuthenticated && router.route.name !== "register") {
    setTimeout(() => {
      router.navigateTo({
        name: "register",
        query: {
          from: location.href,
        },
      });
    }, 0);
  }

  let needsMiscTab = $derived(
    !["bloggy/home", "bloggy/trending", "bloggy/score"].includes(
      router.route.name,
    ),
  );
  let navBorderColor = $derived.by(() => {
    switch (router.route.name) {
      case "bloggy/home":
        return "border-blue-800";
      case "bloggy/trending":
        return "border-purple-800";
      case "bloggy/score":
        return "border-green-800";
      default:
        return "border-red-800";
    }
  });
  let miscTabSnippet = $derived(
    (router.meta.tabName as Snippet) ?? defaultMiscTab,
  );
</script>

{#snippet defaultMiscTab()}
  Misc
{/snippet}

<div class="flex flex-col min-h-dvh">
  <header
    class="sticky top-0 backdrop-blur-md backdrop-grayscale-50 drop-shadow-md"
  >
    <div class="max-w-[640px] m-auto">
      <div class="flex items-center justify-between gap-12 px-4 py-2">
        <Link route={{ name: "bloggy/home" }} class="h-8">
          <Logo format="short" class="w-auto h-full fill-gray-900" />
        </Link>
        <Search />
        {#if auth.user}
          <div
            class="h-8 w-8 rounded-full overflow-clip border-2 border-solid border-gray-100 drop-shadow-sm"
          >
            <!-- TODO: link to user page -->
            <img src={auth.user.pfpUrl} alt="your pfp" />
          </div>
        {/if}
      </div>
    </div>
  </header>

  <div class="grow flex flex-col bg-gray-300">
    <div class="grow max-w-[640px] w-dvw m-auto bg-white">
      <RouterOutlet />
    </div>
  </div>

  <div class="sticky bottom-0 bg-gray-300">
    <div
      class="max-w-[640px] w-dvw m-auto flex flex-col-reverse overflow-x-scroll border-t-2 border-solid {navBorderColor}"
    >
      <!-- TODO top border of nave should be color of active route -->
      <nav
        class="flex flex-row-reverse justify-end px-2 sm:px-0 pb-2"
      >
        <!-- Navigation tabs -->
        <!-- Tabs are in reverse order so the first tab renders above subsequent tabs -->

        {#if needsMiscTab}
          <div
            class="c-tab border-l-6 border-solid border-red-800 py-1 pl-2 pr-3"
          >
            <Link class="no-underline!" route={router.route}>
              {@render miscTabSnippet()}
            </Link>
          </div>
        {/if}
        <div
          class="c-tab border-l-6 border-solid border-green-800 py-1 pl-2 pr-3"
        >
          <Link class="no-underline!" route={{ name: "bloggy/score" }}>
            <Trophy /> Score
          </Link>
        </div>
        <div
          class="c-tab border-l-6 border-solid border-purple-800 py-1 pl-2 pr-3"
        >
          <Link class="no-underline!" route={{ name: "bloggy/trending" }}>
            <Firework /> Trending
          </Link>
        </div>
        <div
          class="c-tab border-l-6 border-solid border-blue-800 py-1 pl-2 pr-3"
        >
          <Link class="no-underline!" route={{ name: "bloggy/home" }}>
            <Notebook /> Homeroom
          </Link>
        </div>
      </nav>
    </div>
  </div>

  <Footer />
</div>

<style lang="css">
  @reference "../app.css";

  .c-tab {
    @apply shrink-0 relative bg-gray-100 shadow-md rounded-b-lg;
  }

  .c-tab:has(:global(a.active)) {
    @apply bg-white z-10;
  }

  .c-tab :global(a) {
    @apply text-gray-700;
  }

  .c-tab :global(a.active) {
    @apply text-black;
  }
</style>
