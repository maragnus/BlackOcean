import resolve from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import terser from '@rollup/plugin-terser';
import typescript from '@rollup/plugin-typescript';
import minifyHTML from 'rollup-plugin-minify-html-literals';
import copy from 'rollup-plugin-copy';
import sass from 'rollup-plugin-sass';

sass({ api: 'modern' });

// `npm run build` -> `production` is true
// `npm run dev` -> `production` is false
const production = !process.env.ROLLUP_WATCH;

// Static assets will vary depending on the application
const copyConfig = {
  targets: [
    { src: 'src/public/*', dest: 'build' },
  ],
};

const sassConfig = {
  output: 'build/bundle.css',
  api: 'modern'
}

export default {
  input: 'src/index.ts',
  treeshake: false,
  output: {
    format: 'iife',
    file: 'build/bundle.js',
//    format: 'cjs',
//    dir: 'build',
  },
  
  plugins: [
    sass(sassConfig),
    minifyHTML(),
    copy(copyConfig),
    typescript(),
    resolve(), // tells Rollup how to find date-fns in node_modules
    commonjs({sourceMap: false}), // converts date-fns to ES modules
    production && terser() // minify, but only in production
  ],
  preserveEntrySignatures: false,
};