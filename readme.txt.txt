������ �������� � Visual Studio 2015.
������ ����� ������������� ����������:
 - Core: �������� ���������
 - BusinessLogic: ������ �������������� � �� (�������: �����������, ��������, ������������)
 - Web: ���������� Web-����������.

����������� ������� �������:
- ���� ����� � �������
- ����� ��������� �������� �� ��������� ���������
- ��������� ����� � ���� ����� ����������� ��������
- ����� ����� �����
�����������������:
- ��������/��������������/��������/������������ �����
- ��������/��������������/�������� ��������
- ������ � ��������� �� ���������� �����
- ������ �������� � ����� .xml

��� �������������� ���������� ���������:
�	��� �������� ����� ���������� ���������� � ������� �����. 
�	����������� ������� ��������.

��������� ������������ �������������� ��� ������ Ninject.

�����������:
MVC: HomeController, AdminController
WebApi: BaseApiController, CoinController, DrinkController, UploadController

MyHelpers - htmlHelper ��� ����������� �����

������ � ���������������� ��������� �������������� �� ���������� �����, �������� � web.config.
<add key="Admin" value="adminpassword"/>
���������� � �������� ��������� � �������� ������:
http:\localhost:****\admin?adminpassword

Scripts:
indexHome.js ��� \Home\Index
indexAdmin.js ��� \Admin\Index
manageCoin.js ��� PartialView _ManageCoin  (��������� ����)
manageDrink.js ��� PartialView _ManageDrink  (��������� ����)
change.js ��� PartialView _Change  (��������� ����)

�� ����� ������ .mdf. ��������� ������ ������������. ����������� �� AttachDbFilename.
��� �������� �� ����������� Migrations.

���� ����������:
C#, ASP.NET MVC WebApi, EF, Ninject, JavaScript, jQuery, AJAX, Bootstrap