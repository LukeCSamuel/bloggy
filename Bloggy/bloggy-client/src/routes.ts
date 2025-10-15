import ErrorPage from './lib/error-page.svelte';
import Home from './lib/home.svelte';
import Halloween from './lib/halloween/halloween.svelte';
import Layout from './lib/layout.svelte';
import { AppRouter, match, param } from './lib/router/configure-routes.svelte';

export const router = new AppRouter([
  {
    name: 'invitation',
    match: match`${param('_').match(/\/?/)}`,
    view: Halloween,
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