$(document).ready(function () {
    var alert = $('.alert');

    // Skryjeme upozornění po 3 sekundách
    setTimeout(function () {
      alert.alert('close');
    }, 3000);
  });