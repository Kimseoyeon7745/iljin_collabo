/*����ȭ�� js*/
$(function () {
	var submenu = $('.submenu > ul > li'),
		depth02 = $('.submenu .depth02');
	var selectedSubItem = document.getElementById('selectedSubItem'); //���߿�
	//submenu.find('> a').on('click', function(){  //�ۺ��̿�
	//	var i = $(this).closest('li').index();
	//	submenu.removeClass('on').eq(i).addClass('on');
	//	depth02.removeClass('on').eq(i).addClass('on');
	//});
	
	depth02.find('li').on('click', function(){
		depth02.find('li').removeClass('on');
		$(this).addClass('on');
	});

	submenu.eq(0).find('> a').trigger('click');
	//depth02.find('li').eq(0).trigger('click'); �ۺ��̿�
	depth02.find('li').eq(selectedSubItem.value).trigger('click'); //���߿�
});