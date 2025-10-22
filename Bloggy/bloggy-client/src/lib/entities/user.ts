import { api } from '../utils/api';
import { auth, getCurrentUserAsync } from '../utils/auth.svelte';
import type { EntityBase } from './entity-base';

export interface User extends EntityBase {
  name: string;
  pfpUrl: string;
  created: string;
}

export async function uploadPfpAsync (blob: Blob) {
  if (!auth.user) {
    return;
  }

  await api.postJsonAsync<User>(`api/user/${auth.user.id}/pfp`, blob, 'image/bitmap');
  await getCurrentUserAsync();
}
