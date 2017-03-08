---
layout: docs
title: ruby-locate
group: shades
---

Locates the ruby, gem and bundler command line utilities available on the current path.

## Contents

* Will be replaced with the table of contents
{:toc}

## Supported Operating Systems

{% icon fa-apple fa-3x %} {% icon fa-windows fa-3x %} {% icon fa-linux fa-3x %}

## Arguments

The `ruby-locate` shade does not accept any arguments.

## Global Arguments

The `ruby-locate` shade does not accept any global arguments. It will, however, attempt to find
ruby on a path indicated by the environment variable `${env:RUBY_INSTALL_PATH}` before scanning the
`${env:PATH}`. This is done to enable the use of specific versions of ruby where Ruby DevKit may be
installed (such as on AppVeyor).

## Examples

### Locate Ruby

{% highlight sh %}
ruby-locate
{% endhighlight %}

## See Also

* [ruby]({{site.baseurl}}/shades/ruby)
* [gem]({{site.baseurl}}/shades/gem)
* [gem-install]({{site.baseurl}}/shades/gem-install)
* [bundle]({{site.baseurl}}/shades/bundle)
* [bundle-download]({{site.baseurl}}/shades/bundle-download)
* [bundle-install]({{site.baseurl}}/shades/bundle-install)