   M S B A M L   `   `   `  ����   PMA_Project    PMA_Project.View.Settings   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  系统配置��$	��480��$	��420�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ 
ToolWindow=� � 
ResizeMode$ NoResize=�.��� $��10q� � RowDefinitions �� $
��auto ��� $
��1.5* ��� $
��auto ��� $��* ��� $��* ��� $
��auto ��� $��* ��� $��* ��� $
��1.5* ��� $
��1.5* ��� $��* �.+���  �� Orientation$ 
Horizontal=�.+��� $��接口卡类型：���� -    ��Name$ 
cbxDevType��$	��150��
������ .@��� .��Vector VN1630�� .��Vector VN1640A�� .��	PEAK PCAN�� .��
ValueCAN�� .��周立功白CAN盒�� $��模式选择��$��1��$	��0,5q�.���� $ 
Horizontal=�$��Center=�.+�	�  	� 	GroupName$ mode1��$��通用模式�� d� 	IsChecked$
 TrueY�	� $ mode1��$��诊断模式��$ falseY�$	��5,0q�{�.  �� -   	 ��Name$	 chxChannel01��$��2��$��通道1��$��0,0,0,5q��� $��3��$��2�����	#FFD5DFE5$��1q�
 �� CornerRadius$
 3��]� -    ]�Name$ dpChannel01��$��3��$��2��{�.  .+��� $	��Top=�$��6��$ 
Horizontal=�$��Center=�$��5q�.+��� $��	波特率���� -    ��Name$ tbxBauRat01��$	��150��$	��3,0q�$	��500��{���  �� Path$ 	IsEnabled� �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType" ]� �� AncestorLevel$ 2���� $
��kbps���� $��7��$ 
Horizontal=�$��Center=�$��5q�.+��� $��	DBC文件���� -   $ tbDBCFilePath01��$	��250��$	��3,0q�{��� $ 	IsEnabled� �� $ FindAncestor=�" ]�$ 2���� -    ��Name$ 	btnOpen01��$��浏览��$	��5,0q�$��50��
������ ���	#FFD49A08{��� $ 	IsEnabled� �� $ FindAncestor=�" ]�$ 2���� -   $	 chxChannel02��$��5��$��通道2��$	��0,5q�{�. �� $��6��$��2�����	#FFD5DFE5$��1q�$
 3��]� -	   $ dpChannel02��$��6��$��2��{�.  .+��� $	��Top=�$��6��$ 
Horizontal=�$��Center=�$��5q�.+��� $��	波特率���� -
   $ tbxBauRat02��$	��150��$	��3,0q�$	��500��{��� $ 	IsEnabled� �� $ FindAncestor=�" ]�$ 2���� $
��kbps���� $��7��$ 
Horizontal=�$��Center=�$��5q�.+��� $��	DBC文件���� -   $ tbDBCFilePath02��$	��250��$	��3,0q�{��� $ 	IsEnabled� �� $ FindAncestor=�" ]�$ 2���� -   $ 	btnOpen02��$��浏览��$	��5,0q�$��50��
������ ���	#FFD49A08{��� $ 	IsEnabled� �� $ FindAncestor=�" ]�$ 2���� $��诊断通道选择��$��8��{�.  
�������$	��0,5q�.���� $ 
Horizontal=�$��Center=�.+��� $��通道1��$ FalseY��� $��通道2��$ falseY�$	��5,0q�{�. �� $'��!诊断物理和功能地址设置��$��9��{�.  
�������.���� $ 
Horizontal=�$��Center=�.+��� $��物理ID：0x���� $��50��$	��3,0q��� $��功能ID：0x��$��10,0,0,0q��� $��50��$	��3,0q��� $��接收ID：0x��$��10,0,0,0q��� $��50��$	��3,0q��� $��10��$ 
Horizontal=�$��Center=�$��Right=�.+��� -   $ btnOK��$��确认��$	��5,0q�$��50��
������ ���	#FFD49A08�� -   $ 	btnCancel��$��取消��$	��5,0q�$��50��
������ ���	#FFD49A08