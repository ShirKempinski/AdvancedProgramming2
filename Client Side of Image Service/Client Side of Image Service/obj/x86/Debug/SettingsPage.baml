   M S B A M L   `   `   `  ����W  SClient Side of Image Service, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:Client_Side_of_Image_ServiceClient_Side_of_Image_Service  5      /    )Client_Side_of_Image_Service.SettingsPage   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:Client_Side_of_Image_Service     5      .��5	      � -   5          �Name$  SettingsView��5	      .+�5
   
   �� $��1��6   $��3��5      ��5      �� *!(FrameworkElement.ActualHeight)+ �� RelativeSource ��  �� AncestorType$ UserControlL�.��5      �  � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$    � 	Resources 5       	  title �� Property �� Value��%�   &	        �� 5      .�5      ��  �@��Padding$ 3q�6>   �� 5       � ��$ 3q�6=   �� 5       � ��
 �����6A   5      5      .+�5      �� $��0��6   
�������._�5      � $��5q�6    5      �� 5      $
��Auto �6,   �� 5      $
��Auto �6,   �� 5      $
��Auto �6,   �� 5       $
��Auto �6,   �� 5!      $
��Auto �6,   �� 5"      $
��Auto �6,   �� 5#      $
��Auto �6,    � ColumnDefinitions 5%      �� 5&      $
��Auto �6/   �� 5'      $
��Auto �6/   .+�5)      �� $��0��6$   $��0��5*   $   $��Bottom=�5+   $   
���������6R   Z� �� 5,      $�Output Directory��6"   #��[  6:   
�������
���   �6l   5-      �� 5.      -   5         ��Name$ OutputDirectory��5.   1   $��0��6$   $	��Top=�5/   $   	 J� RenderTransformOrigin$	 0.837,0.284&�50   $   $��1��6H   $��6.667,0,-6.667,0q�6X   �� 51      
�������
���   ��6"   �� *outputDirectory.entry+
 �� Mode$
 OneWay=�6i   52      �� 54      $��0��6$   $��1��55   $   $��Bottom=�56   $   �� 6@   $�Source Name��6D   #��[  6W   
���   �6v   57      �� 58      -   5        $ 
SourceName��58   4   $��1��6$   $��Bottom=�59   $   $��2��5:   $   $��6.667,0,-6.667,0q�68   $��1��6R   �� 5;      
�������
���   ��6"   �� *sourceName.entry+$
 OneWay=�6e   5<      �� 5>      $��0��6$   $��2��5?   $   $��Bottom=�5@   $   �� 6@   $�Log Name��6D   #��[  6T   
���   �6s   5A      �� 5B      -   5        $ LogName��5B   1   $��2��6$   $	��Top=�5C   $   $	 0.837,0.284&�5D   $   $��1��6H   $��6.667,0,-6.667,0q�6X   �� 5E      
�������
���   ��6"   �� *logName.entry+$
 OneWay=�6a   5F      �� 5H      $��0��6$   $��3��5I   $   $��Bottom=�5J   $   �� 6@   $�Thumbnail Size��6D   #��[  6Z   
���   �6y   5K      �� 5L      -   5        $ ThumbnailSize��5L   1   $��3��6$   $	��Top=�5M   $   $	 0.837,0.284&�5N   $   $��1��6H   $��6.667,0,-6.667,0q�6X   �� 5O      
�������
���   ��6"   �� *thumbnailSize.entry+$
 OneWay=�6g   5P      �� 5Q      -   5         ��Name$ RemoveDirButton��5R   *   $��5��6   $��Remove��5T      4  .System.Windows.Automation.AutomationProperties   AutomationId$ RemoveDirButton��5U      $��1��6Q   $��219,0,-314,0q�6a   $	 
1.49,0.348&�5V      $��2��6@    � Cursor$ Arrow��6Q   ��5S      �� *RemoveCommand+5V   Q   �� 5X      $��1��6!   ._�5Y      �� -   5         ��Name$ handlersBox��5Y   &   ��.  $��-5,84,-705,-401q�5]   "   ��5[   "   �� *
handlers+ �� SelectedItem 5\   "   �� *ChosenHandler+$
 TwoWay=�5^      5_      5`      5a      5b      5c      5d      5f      