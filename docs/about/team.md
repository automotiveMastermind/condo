---
layout: docs
title: Team
group: about
---

> PulseBridge Condo is maintained by the PulseBridge team and its contributors.

<div class="list-group docs-team">
  {% for member in site.data.team %}
    <div class="list-group-item">
      <iframe class="github-btn" src="https://ghbtns.com/github-btn.html?user={{ member.user }}&amp;type=follow"></iframe>
      <a class="team-member" href="https://github.com/{{ member.user }}">
        <img src="https://secure.gravatar.com/avatar/{{ member.gravatar }}" alt="@{{ member.user }}" width="32" height="32">
        <strong>{{ member.name }}</strong> <small>@{{ member.user }}</small>
      </a>
    </div>
  {% endfor %}
</div>

Get involved with Condo development by [opening an issue](https://github.com/pulsebridge/condo/issues/new) or submitting a pull request. Read our [contributing guidelines](https://github.com/pulsebridge/condo/blob/master/CONTRIBUTING) for information on how we work together.
