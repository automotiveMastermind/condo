import * as gulp from 'gulp';
import * as del from 'del';
import * as maps from 'gulp-sourcemaps';
import * as sass from 'gulp-sass';
import * as concat from 'gulp-concat';
import * as js from 'gulp-uglify';
import { exec } from 'child_process';
import * as util from 'gulp-util';
import * as cache from 'gulp-cached';
import * as mem from 'gulp-remember';
import * as sync from 'browser-sync';

import pages = require('gulp-gh-pages');
import css = require('gulp-minify-css');
import rename = require('gulp-rename');
import html = require('gulp-minify-html');
import err = require('gulp-plumber');
import prefix = require('gulp-autoprefixer');

const sequence = require('run-sequence').use(gulp);
const browser = sync.create();

var prod:boolean = true;

var server = {
    server: {
        baseDir: '.docs',
        routes: <{ [x: string] }>{
            "/condo" : ".docs"
        }
    },
    open: <any>'local'
};

gulp.task('styles:vnd:bootstrap', () => {
    return gulp
        .src('bower_components/bootstrap/scss/**/*.scss')
        .pipe(gulp.dest('docs/assets/scss/vnd/bootstrap'));
});

gulp.task('styles:vnd:font-awesome', () => {
    return gulp
        .src('bower_components/font-awesome/scss/*.scss')
        .pipe(gulp.dest('docs/assets/scss/vnd/font-awesome'));
});

gulp.task('styles:vnd', (done:any) => sequence([
    'styles:vnd:bootstrap',
    'styles:vnd:font-awesome'
], done));

gulp.task('styles:compile', () => {
    return gulp
        .src('docs/assets/scss/docs.scss')
        .pipe(err())
        .pipe(prod ? util.noop() : maps.init())
        .pipe(<any>sass().on('error', sass.logError))
        .pipe(prefix({ browsers: ['> 5%', 'last 2 versions'] }))
        .pipe(prod ? css() : util.noop())
        .pipe(prod ? rename({ suffix: '.min', extname: '.css' }) : util.noop())
        .pipe(prod ? util.noop() : maps.write('.'))
        .pipe(gulp.dest('.docs/assets/css'))
        .pipe(prod ? util.noop() : browser.stream())
        .pipe(err.stop());
});

gulp.task('scripts:vnd:jquery', () => {
    return gulp
        .src('bower_components/jquery/dist/jquery.min.js')
        .pipe(gulp.dest('docs/assets/js/vnd/jquery'))
        .pipe(gulp.dest('.docs/assets/js'));
});

gulp.task('scripts:vnd:bootstrap', () => {
   return gulp
        .src('bower_components/bootstrap/dist/js/bootstrap.js')
        .pipe(gulp.dest('docs/assets/js/vnd/bootstrap'));
});

gulp.task('scripts:vnd:anchor-js', () => {
    return gulp
        .src('bower_components/anchor-js/anchor.js')
        .pipe(gulp.dest('docs/assets/js/vnd/anchor-js'));
});

gulp.task('scripts:vnd:clipboard', () => {
    return gulp
        .src('bower_components/clipboard/dist/clipboard.js')
        .pipe(gulp.dest('docs/assets/js/vnd/clipboard'));
});

gulp.task('scripts:vnd:tether', () => {
    return gulp
        .src('bower_components/tether/dist/js/tether.js')
        .pipe(gulp.dest('docs/assets/js/vnd/tether'));
});

gulp.task('scripts:vnd:lunr', () => {
    return gulp
        .src('node_modules/lunr/lunr.min.js')
        .pipe(gulp.dest('docs/assets/js/vnd/lunr'))
        .pipe(gulp.dest('.docs/assets/js'));
});

gulp.task('scripts:vnd:picturefill', () => {
    return gulp
        .src('bower_components/picturefill/dist/picturefill.min.js')
        .pipe(gulp.dest('docs/assets/js/vnd/picturefill'))
        .pipe(gulp.dest('.docs/assets/js'));
});


gulp.task('scripts:vnd', (done:any) => sequence([
    'scripts:vnd:lunr',
    'scripts:vnd:jquery',
    'scripts:vnd:bootstrap',
    'scripts:vnd:anchor-js',
    'scripts:vnd:clipboard',
    'scripts:vnd:tether',
    'scripts:vnd:picturefill'
], done));

gulp.task('scripts:compile', () => {
    const src = [
        'docs/assets/js/vnd/tether/*.js',
        'docs/assets/js/vnd/clipboard/*.js',
        'docs/assets/js/vnd/anchor-js/*.js',
        'docs/assets/js/vnd/bootstrap/*.js',
        'docs/assets/js/*.js',
        '!docs/assets/js/search*.js'];

    return gulp
        .src(src)
        .pipe(err())
        .pipe(prod ? util.noop() : maps.init())
        .pipe(prod ? js() : util.noop())
        .pipe(concat(prod ? 'docs.min.js' : 'docs.js'))
        .pipe(prod ? util.noop() : maps.write('.'))
        .pipe(gulp.dest('.docs/assets/js'))
        .pipe(prod ? util.noop() : browser.stream())
        .pipe(err.stop());
});

gulp.task('scripts:search', () => {
    const src = [
        'docs/assets/js/search*.js'
    ];

    return gulp
        .src(src)
        .pipe(err())
        .pipe(prod ? js() : util.noop())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest('.docs/assets/js'))
        .pipe(err.stop());
});

gulp.task('fonts:vnd:font-awesome', () => {
    return gulp
        .src('bower_components/font-awesome/fonts/fontawesome-*')
        .pipe(gulp.dest('.docs/assets/fonts'));
});

gulp.task('fonts:vnd', (done:any) => sequence('fonts:vnd:font-awesome', done));

gulp.task('html:build', (done:any) => {
    var jekyll = 'jekyll build';

    if (!prod) {
        jekyll += ' --config _config.yml,_config.dev.yml';
    }

    exec(jekyll, (err, stdout, stderr) => {
        console.log(stdout);
        console.log(stderr);

        done(err);
    });
});

gulp.task('html:compile', () => {
    return gulp
        .src('.docs_jekyll/**/*.html')
        .pipe(err())
        .pipe(prod ? html({ cdata: true, conditionals: true }) : util.noop())
        .pipe(gulp.dest('.docs'))
        .pipe(err.stop());
});

gulp.task('html:static', () => {
    return gulp
        .src('.docs_jekyll/*')
        .pipe(gulp.dest('.docs'));
})

gulp.task('dev', () => {
    prod = false;
});

gulp.task('no-browse', () => {
    server.open = false;
});

gulp.task('init', () => {
    browser.init(server);
});

gulp.task('img', () => {
    return gulp
        .src('docs/assets/img/**')
        .pipe(gulp.dest('.docs/assets/img'));
});

gulp.task('reload', ['html', 'img'], () => {
    return browser.reload();
});

gulp.task('clean', () => {
    return del(['docs/assets/**/vnd', '.docs', '.docs_jekyll', 'docs/assets/**/docs*.js', 'docs/assets/**/docs*.css', 'docs/assets/**/docs*.map']);
});

gulp.task('styles', (done:any) => sequence('styles:vnd', 'styles:compile', done));
gulp.task('scripts', (done:any) => sequence('scripts:vnd', ['scripts:compile', 'scripts:search'], done));
gulp.task('fonts', (done:any) => sequence('fonts:vnd', done));
gulp.task('html', (done:any) => sequence('html:build', ['html:compile', 'html:static'], done));
gulp.task('docs', (done:any) => sequence('clean', 'html', ['styles', 'scripts', 'fonts', 'img'], done));

gulp.task('sync', (done:any) => {
    gulp.watch('docs/assets/js/*.js', ['scripts:compile', 'scripts:search']).on('deleted', (event) => delete cache.caches['scripts'][event.path]);
    gulp.watch('docs/assets/scss/**/*.scss', ['styles:compile']);
    gulp.watch(['docs/**/*.*', '!docs/**/*.js', '!docs/**/*.scss'], ['reload']);
});

gulp.task('deploy', ['docs'], () => {
    return gulp
        .src(".docs/**/*")
        .pipe(pages());
});

gulp.task('watch', (done:any) => sequence('dev', 'docs', 'init', 'sync', done));
gulp.task('serve', (done:any) => sequence('dev', 'docs', 'no-browse', 'init', done));