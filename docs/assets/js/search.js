$(function () {
  var searchIndex,
      searchHits,
      searchWorker = new Worker("/condo/assets/js/search_worker.min.js");

  if (localStorage['search']) {
    searchIndex = JSON.parse(localStorage['search']);

    if (localStorageHasExpired()) {
      loadSearchIndex();
    }
    else {
      searchIndex.type = "index";
      searchWorker.postMessage(searchIndex);
    }
  } else {
    loadSearchIndex();
  }

  function loadSearchIndex() {
    $.getJSON('/condo/search.json', function(data) {
      data.type = { index: true };
      searchWorker.postMessage(data);
      searchIndex = data;
      localStorage['search'] = JSON.stringify(searchIndex);
      localStorage['updated'] = new Date().getTime();
    });
  }

  function localStorageHasExpired() {
    if (new Date().getTime() - parseInt(localStorage['updated'],10) > 86400000) {
      return true;
    }

    return false;
  }

  function cancelSearch() {
    $("#search-input").val("");
    $("#search-container").removeClass("active");
  }


  if ($("#search-input").val().length > 0) {
    searchForString($("#search-input").val());
  }

  $("#search-input").on("input", function(e) {
    $(this).val().length > 0 ? $("#search-container").addClass("active") : $("#search-container").removeClass("active");

    searchForString($(this).val());
  });

  $("body").keyup(function(e) {
    if (e.keyCode == 83) {
      // S key
      if ($("#search-input").is(":focus"))
        return;

      e.preventDefault();
      $("#search-input").focus();
    }
  });

  $("#search-input").keyup(function(e) {
    if (e.keyCode == 27) {
      e.preventDefault();
      $("#search-input").val().length > 0 ? cancelSearch() : $("#search-input").blur();
    } else if (e.keyCode == 13) {
      e.preventDefault();
      goToSelectedSearchResult();
    }  else if (e.keyCode == 8 || e.keyCode == 46) {
      $(this).val().length > 0 ? $("#search-container").addClass("active") : $("#search-container").removeClass("active");

      searchForString($(this).val());
    }
  }).keydown(function(e) {
    if (e.keyCode == 38) {
      e.preventDefault();
      moveSearchSelectionUp();
    } else if (e.keyCode == 40) {
      e.preventDefault();
      moveSearchSelectionDown();
    } else if (e.keyCode == 27) {
      e.preventDefault();
    }
  });

  function searchForString(searchString) {
    searchHits = [];
    searchString = searchString.toLowerCase();
    searchWorker.postMessage({ query: searchString, type: "search" })
  }

  searchWorker.addEventListener("message", function (e) {
    if (e.data.type.search) {
      renderResultsForSearch(e.data.query, e.data.results);
    }
  });

  function renderResultsForSearch(searchString, searchHits){
    $("#search-results").empty();

    if (searchHits.length < 1) {
      $('<a class="placeholder">No results for <em></em></a>').appendTo("#search-results").find("em").text(searchString);
      return;
    }

    for (var i = 0; i < Math.min(searchHits.length, 8); i++) {
      var page = searchHits[i];

      $('<a href="' + page.url + '"><em>' + page.title + '</em></a>').appendTo("#search-results");
    }

    $("#search-results a:first-child").addClass("selected");
  }

  $("#search-results").on("mouseenter", "a", function(e) {
    $(this).parent().find(".selected").removeClass("selected").end().end()
      .addClass("selected");
  });

  function moveSearchSelectionUp() {
    $prev = $("#search-results .selected").prev();
    if ($prev.length < 1)
      return;

    $("#search-results .selected").removeClass("selected");
    $prev.addClass("selected");
  }

  function moveSearchSelectionDown() {
    $next = $("#search-results .selected").next();
    if ($next.length < 1)
      return;

    $("#search-results .selected").removeClass("selected");
    $next.addClass("selected");
  }

  function goToSelectedSearchResult() {
    var href = $("#search-results .selected").attr("href");
    if (href)
      window.location.href = href;
  }
});