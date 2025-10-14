import ErrorPage from './lib/error-page.svelte';
import Home from './lib/home.svelte';
import Invitation from './lib/invitation.svelte';
import Layout from './lib/layout.svelte';
import { AppRouter, match, param } from './lib/router/configure-routes.svelte';

export const router = new AppRouter([
  {
    name: 'invitation',
    match: match`${param('_').match(/\/?/)}`,
    view: Invitation,
  },
  {
    name: 'bloggy',
    match: match`/bloggy`,
    view: Layout,
    nested: [{
      name: 'home',
      match: match`${param('_').match(/\/?/)}`,
      view: Home,
    }],
  },
  {
    name: 'error',
    match: match`${param('path').match(/.*/)}`,
    view: ErrorPage,
  },
]);