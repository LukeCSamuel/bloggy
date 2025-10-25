/* eslint-disable unicorn/no-useless-undefined */
import type { User } from '../entities/user';
import { api, TOKEN_KEY } from './api';

interface RegisterDto {
  accessToken: string;
  expiresAt: string;
};

let user: User | undefined = $state(undefined);

class UserState {
  readonly isAuthenticated = $derived(!!user);
  readonly user = $derived(user);
}

export const auth = new UserState();

export async function getCurrentUserAsync () {
  try {
    return user = await api.getJsonAsync<User>('api/auth/current-user');
  } catch {
    // user couldn't be deserialized
  }
}

export async function registerAsync (body: Pick<User, 'name'>): Promise<boolean> {
  const { accessToken } = await api.postJsonAsync<RegisterDto>('api/auth/register', JSON.stringify(body)) ?? {};

  if (accessToken) {
    localStorage.setItem(TOKEN_KEY, accessToken);
    await getCurrentUserAsync();
    return true;
  } else {
    return false;
  }
}

await getCurrentUserAsync();
