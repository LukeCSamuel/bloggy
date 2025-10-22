<script lang="ts">
  import Logo from "./icons/logo.svelte";
  import { getRouter } from "./router/configure-routes.svelte";
  import Link from "./router/link.svelte";
  import RouterOutlet from "./router/router-outlet.svelte";
  import { auth } from "./utils/auth.svelte";

  // TODO: when hitting the bloggy layout, check that the user is logged in
  //       if not, redirect them to the account creation page

  const router = getRouter();

  if (!auth.isAuthenticated && router.route.name !== 'bloggy/register') {
    setTimeout(() => {
      router.navigateTo({
        name: 'bloggy/register',
        query: {
          from: location.href,
        },
      })
    }, 0);
  }
</script>

<header
  class="sticky top-0 flex items-center justify-between px-4 py-2 backdrop-blur-md"
>
  <Link route={{ name: "bloggy/home" }} class="h-8">
    <Logo format="short" class="w-auto h-full fill-gray-900" />
  </Link>
  <!-- TODO: search component -->
  {#if auth.user}
    <div class="h-8 w-8 rounded-full overflow-clip">
      <!-- TODO: link to user page -->
      <img src={auth.user.pfpUrl} alt="your pfp">
    </div>
  {/if}
</header>

<RouterOutlet />
