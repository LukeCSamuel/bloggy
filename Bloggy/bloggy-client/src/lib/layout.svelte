<script lang="ts">
    import Search from './components/search.svelte';
  import Logo from "./icons/logo.svelte";
  import { getRouter } from "./router/configure-routes.svelte";
  import Link from "./router/link.svelte";
  import RouterOutlet from "./router/router-outlet.svelte";
  import { auth } from "./utils/auth.svelte";

  // TODO: when hitting the bloggy layout, check that the user is logged in
  //       if not, redirect them to the account creation page

  const router = getRouter();

  if (!auth.isAuthenticated && router.route.name !== "bloggy/register") {
    setTimeout(() => {
      router.navigateTo({
        name: "bloggy/register",
        query: {
          from: location.href,
        },
      });
    }, 0);
  }
</script>

<header class="sticky top-0 px-4 py-2 backdrop-blur-md drop-shadow-md">
  <div class="max-w-[640px] m-auto flex items-center justify-between gap-12">
    <Link route={{ name: "bloggy/home" }} class="h-8">
      <Logo format="short" class="w-auto h-full fill-gray-900" />
    </Link>
    <Search />
    {#if auth.user}
      <div class="h-8 w-8 rounded-full overflow-clip border-2 border-solid border-gray-100 drop-shadow-sm">
        <!-- TODO: link to user page -->
        <img src={auth.user.pfpUrl} alt="your pfp" />
      </div>
    {/if}
  </div>
</header>

<div class="max-w-[640px] m-auto">
  <RouterOutlet />
</div>
