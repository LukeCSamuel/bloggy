<script lang="ts">
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

  // TODO: when hitting the bloggy layout, check that the user is logged in
  //       if not, redirect them to the account creation page

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
</script>

<div class="flex flex-col min-h-dvh">
  <header class="sticky top-0 backdrop-blur-md drop-shadow-md">
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

  <div class="grow max-w-[640px] w-dvw m-auto">
    <RouterOutlet />
  </div>

  <div class="bg-gray-300">
    <div class="max-w-[640px] w-dvw m-auto">
      <!-- TODO top border of nave should be color of active route -->
      <nav
        class="sticky bottom-0 flex flex-row-reverse justify-end overflow-x-scroll px-2 pb-2"
      >
        <!-- Navigation tabs -->
        <!-- Tabs are in reverse order so the first tab renders above subsequent tabs -->

        <!-- TODO: if viewing a route that isn't one of these, add a 4th tab for that specific route -->
        <div class="c-tab border-l-6 border-solid border-l-green-800 py-1 px-2">
          <Link class="no-underline!" route={{ name: "bloggy/score" }}>
            <Trophy /> Score
          </Link>
        </div>
        <div
          class="c-tab border-l-6 border-solid border-l-purple-800 py-1 px-2"
        >
          <Link class="no-underline!" route={{ name: "bloggy/trending" }}>
            <Firework /> Trending
          </Link>
        </div>
        <div class="c-tab border-l-6 border-solid border-l-blue-800 py-1 px-2">
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
    @apply relative bg-gray-100 shadow-md rounded-b-lg;
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
