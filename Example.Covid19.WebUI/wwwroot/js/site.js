$(function () {
  var byCountryDateFrom = document.getElementById("byCountryDateFrom");
  var byCountryDateTo = document.getElementById("byCountryDateTo");
  var byCountryLiveDateFrom = document.getElementById("byCountryLiveDateFrom");
  var byCountryLiveDateTo = document.getElementById("byCountryLiveDateTo");
  var byCountryTotalDateFrom = document.getElementById("byCountryTotalDateFrom");
  var byCountryTotalDateTo = document.getElementById("byCountryTotalDateTo");
  var liveByCountryAndStatusAfterDate_Date = document.getElementById("liveByCountryAndStatusAfterDate_Date");

  var date = new Date();
  var day = date.getDate();
  var month = date.getMonth() + 1;
  var year = date.getFullYear();

  if (month < 10) month = "0" + month;
  if (day < 10) day = "0" + day;

  var today = year + "-" + month + "-" + day;

  byCountryDateFrom.value = today;
  byCountryDateTo.value = today;
  byCountryLiveDateFrom.value = today;
  byCountryLiveDateTo.value = today;
  byCountryTotalDateFrom.value = today;
  byCountryTotalDateTo.value = today;
  liveByCountryAndStatusAfterDate_Date.value = today;
});