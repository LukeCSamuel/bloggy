import ErrorPage from './lib/error-page.svelte';
import Home from './lib/pages/home.svelte';
import Halloween from './lib/halloween/halloween.svelte';
import Layout from './lib/layout.svelte';
import SupportLayout from './lib/support/layout.svelte';
import { AppRouter, match, param } from './lib/router/configure-routes.svelte';
import Privacy from './lib/support/privacy.svelte';
import CommunityGuidelines from './lib/support/community-guidelines.svelte';
import Terms from './lib/support/terms.svelte';
import Register from './lib/pages/register.svelte';
import About from './lib/support/about.svelte';

export const router = new AppRouter([
  {
    name: 'invitation',
    match: '/',
    view: Halloween,
  },
  {
    name: 'support',
    match: '/bloggy/support',
    view: SupportLayout,
    nested: [
      {
        name: 'privacy',
        match: '/privacy',
        view: Privacy,
      },
      {
        name: 'community-guidelines',
        match: '/community-guidelines',
        view: CommunityGuidelines,
      },
      {
        name: 'terms',
        match: '/terms',
        view: Terms,
      },
      {
        name: 'about',
        match: '/about',
        view: About,
      },
    ],
  },
  {
    name: 'bloggy',
    match: '/bloggy',
    view: Layout,
    nested: [
      {
        name: 'home',
        match: match`${param('_').match(/\/?/)}`,
        view: Home,
      },
      {
        name: 'register',
        match: '/register',
        view: Register,
      },
    ],
  },
  {
    name: 'error',
    match: match`${param('path').match(/.*/)}`,
    view: ErrorPage,
  },
]);
