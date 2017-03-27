$(document).ready(function () {
   /* $('a[data-linkedId]').on('click', function () {
        var divId = $(this).attr('data-linkedId');
       // $('#' + divId).slideToggle("fast");
	  // alert($('#' + divId).addClass('height'));
	   $('#' + divId).toggleClass("height");
		//$('.project-hover').addClass('height');
        return false;
    }); 
	*/
	
	
	   $('a.details').click(function(){
		   $('.project-hover').toggleClass('fliph');
	   });
	  
});