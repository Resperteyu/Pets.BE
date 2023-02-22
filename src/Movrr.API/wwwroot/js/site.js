// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

jQuery(document).ready(function(){
  $('.togglePassword').on('click', function(){ 
    this.classList.toggle('fa-eye-slash');    
    $(this).prev().attr('type', function(index, attr){return attr == 'password' ? 'text' : 'password'; });  
  });  
});