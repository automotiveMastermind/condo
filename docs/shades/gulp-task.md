---
layout: docs
title: gulp-task
group: shades
---

@{/*

gulp
    Executes a gulp task runner command.

gulp_task_name=''
    Required. The task to execute with the gulp command line tool.

gulp_options='' (Environment Variable: GULP_OPTIONS)
    Additional options to pass to the gulp command.

gulp_task_path='$(base_path)'
    The path in which to execute gulp.

base_path='$(CurrentDirectory)'
    The base path in which to execute gulp.

working_path='$(base_path)'
    The working path in which to execute gulp.

gulp_task_wait='true'
    A value indicating whether or not to wait for exit.

gulp_task_quiet='$(Build.Log.Quiet)'
    A value indicating whether or not to avoid printing output.

*/}

use namespace = 'System'
use namespace = 'System.IO'
use namespace = 'System.Linq'

use import = 'Condo.Build'

default base_path       = '${ Directory.GetCurrentDirectory() }'
default working_path    = '${ base_path }'

default gulp_task_name  = 'default'
default gulp_task_path  = '${ working_path }'
default gulp_task_wait  = '${ true }'
default gulp_task_quiet = '${ Build.Log.Quiet }'

gulp-list gulp_list_name='${ gulp_task_path }' gulp_list_path='${ gulp_task_path }'

@{
    Build.Log.Header("gulp-task");

    if (!string.IsNullOrEmpty(gulp_task_name))
    {
        gulp_task_name = gulp_task_name.Trim();
    }

    Build.Log.Argument("task", gulp_task_name);
    Build.Log.Argument("path", gulp_task_path);
    Build.Log.Argument("wait", gulp_task_wait);
    Build.Log.Argument("quiet", gulp_task_quiet);
    Build.Log.Header();

    var gulp_task_exec = string.Empty;

    var gulp_task_requested = gulp_task_name.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

    var gulp_task_list = Build.GetPath(gulp_task_path);

    if (gulp_task_list.Global)
    {
        var gulp_task_available = gulp_task_list.Path.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
        var gulp_task_intersect = gulp_task_requested.Intersect(gulp_task_available);
        var gulp_task_missing = gulp_task_requested.Except(gulp_task_intersect);

        foreach (var gulp_task_current in gulp_task_missing)
        {
            if (string.Equals(gulp_task_current, "default"))
            {
                continue;
            }

            Build.Log.Warn(string.Format("gulp: the task {0} is not defined for the specified path {1} -- the task will not be executed.", gulp_task_current, gulp_task_path));
        }

        gulp_task_exec = string.Join(" ", gulp_task_intersect);
    }
}

gulp gulp_args='${ gulp_task_exec }' gulp_wait='${ gulp_task_wait }' gulp_quiet='${ gulp_task_quiet }' gulp_path='${ gulp_task_path }' if='!string.IsNullOrEmpty(gulp_task_exec)'