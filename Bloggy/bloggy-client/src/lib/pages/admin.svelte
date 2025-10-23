<script lang="ts">
  import { auth } from '../utils/auth.svelte';
</script>

<p>
  User ID:
  <code>{auth.user?.id}</code>
</p>