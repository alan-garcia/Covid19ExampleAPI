$(function () {
  var date = new Date();
  var day = date.getDate();
  var month = date.getMonth() + 1;
  var year = date.getFullYear();

  if (month < 10) month = "0" + month;
  if (day < 10) day = "0" + day;

  var today = year + "-" + month + "-" + day;

  if (document.getElementById("byCountryDateFrom") !== null) {
    var byCountryDateFrom = document.getElementById("byCountryDateFrom");
    byCountryDateFrom.value = today;
  }

  if (document.getElementById("byCountryDateTo") !== null) {
    var byCountryDateTo = document.getElementById("byCountryDateTo");
    byCountryDateTo.value = today;
  }

  if (document.getElementById("byCountryLiveDateFrom") !== null) {
    var byCountryLiveDateFrom = document.getElementById("byCountryLiveDateFrom");
    byCountryLiveDateFrom.value = today;
  }

  if (document.getElementById("byCountryLiveDateTo") !== null) {
    var byCountryLiveDateTo = document.getElementById("byCountryLiveDateTo");
    byCountryLiveDateTo.value = today;
  }

  if (document.getElementById("byCountryTotalDateFrom") !== null) {
    var byCountryTotalDateFrom = document.getElementById("byCountryTotalDateFrom");
    byCountryTotalDateFrom.value = today;
  }

  if (document.getElementById("byCountryTotalDateTo") !== null) {
    var byCountryTotalDateTo = document.getElementById("byCountryTotalDateTo");
    byCountryTotalDateTo.value = today;
  }

  if (document.getElementById("liveByCountryAndStatusAfterDate_Date") !== null) {
    var liveByCountryAndStatusAfterDate_Date = document.getElementById("liveByCountryAndStatusAfterDate_Date");
    liveByCountryAndStatusAfterDate_Date.value = today;
  }
});