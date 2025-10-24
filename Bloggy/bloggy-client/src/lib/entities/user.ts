import { api } from '../utils/api';
import { auth, getCurrentUserAsync } from '../utils/auth.svelte';
import type { EntityBase } from './entity-base';

export interface User extends EntityBase {
  name: string;
  pfpUrl: string;
  created: string;
  isNpc?: boolean;
  about?: string;
}

export type UserUpdateDto = Partial<Pick<User, 'name' | 'about' | 'pfpUrl'>>;

export async function uploadPfpAsync (blob: Blob) {
  if (!auth.user) {
    return;
  }

  await api.postJsonAsync<User>(`api/user/${auth.user.id}/pfp`, blob, 'image/bitmap');
  await getCurrentUserAsync();
}

export function getUserAsync (userId: string) {
  return api.getJsonAsync<User>(`api/user/${userId}`);
}

export function updateUserAsync (userId: string, dto: UserUpdateDto) {
  return api.postJsonAsync<User>(`api/user/${userId}`, JSON.stringify(dto));
}
