/* eslint-disable unicorn/no-null */
/* eslint-disable @stylistic/multiline-ternary */
import { defineConfig } from 'vite';
import { svelte } from '@sveltejs/vite-plugin-svelte';
import { fileURLToPath } from 'node:url';
import tailwindcss from '@tailwindcss/vite';

const isDevelopment = process.env.APP_ENVIRONMENT === 'development';

// https://vite.dev/config/
export default defineConfig({
  build: {
    minify: !isDevelopment,
    sourcemap: isDevelopment,
    watch: isDevelopment ? {
      chokidar: {
        // This is necessary because the files aren't in WSL, but the docker container is :(
        usePolling: true,
      },
    } : null,
    outDir: '../wwwroot',
    emptyOutDir: true,
    lib: {
      name: 'main',
      entry: ['src/main.ts'],
      fileName: (_, entryName) => `${entryName}.js`,
      cssFileName: 'main',
      formats: ['es'],
    },
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('src', import.meta.url)),
    },
  },
  plugins: [
    tailwindcss(),
    svelte(),
  ],
});
