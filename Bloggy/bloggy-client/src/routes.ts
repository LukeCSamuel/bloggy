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
import Trending from './lib/pages/trending.svelte';
import Score from './lib/pages/score.svelte';
import Profile from './lib/pages/profile.svelte';
import Admin from './lib/pages/admin.svelte';

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
    name: 'register',
    match: '/bloggy/register',
    view: Register,
  },
  {
    name: 'admin',
    match: '/bloggy/__admin__',
    view: Admin,
  },
  {
    name: 'bloggy',
    match: '/bloggy',
    view: Layout,
    nested: [
      {
        name: 'home',
        match: '/home',
        view: Home,
        aliases: [
          '',
        ],
      },
      {
        name: 'trending',
        match: '/trending',
        view: Trending,
      },
      {
        name: 'score',
        match: '/score',
        view: Score,
      },
      {
        name: 'profile',
        match: match`/profile/${param('userId').matchSegment()}`,
        view: Profile,
      },
    ],
  },
  {
    name: 'error',
    match: match`${param('path').match(/.*/)}`,
    view: ErrorPage,
  },
]);
