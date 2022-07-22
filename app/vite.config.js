import { defineConfig } from "vite";
import { resolve } from 'path';
import dotEnvHTMLPlugin from "vite-plugin-dotenv-in-html";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
	return {
		build: {
			rollupOptions: {
				input: {
					main: resolve(__dirname, 'index.html'),
					empty: resolve(__dirname, 'empty.html')
				}
			}
		},
		plugins: [
			dotEnvHTMLPlugin(mode),
		],
	};
});
