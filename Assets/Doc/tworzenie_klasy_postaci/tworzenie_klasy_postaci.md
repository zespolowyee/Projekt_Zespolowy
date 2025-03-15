# Tworzenie nowej klasy postaci
By *Mikołaj*
[[#1. Utworzenie modelu postaci]]
[[#2. Rigging]]
[[#3. Export do FBX]]
[[#4. Generowanie animacji]]
## 1. Utworzenie modelu postaci
Uruchamiamy blender. [Tutaj jest dobry materiał na YT jak zacząć](https://www.youtube.com/watch?v=O6HQhs-gk50). Wystarczy obejrzeć:
1. **0.00-9.24** (tworzenie postaci low-poly)
2. **40.00-46.00** (kolorowanie)
Ten proces można pominąć, jeżeli mamy już gotowy model np. ze [sketchfab](https://sketchfab.com/). Ja osobiście stamtąd nie biorę całych modeli, bo najczęściej nie pasują stylistycznie do gry lub nie chcą się poprawnie importować do blendera. Ale pobrałem stamtąd np. miecz i tarczę postaci **Knight**.
Poniżej gotowy model klasy **Axeman**.
![[knight_model.png]]
## 2. Rigging
Mamy surowy model, trzeba dodać do niego szkielet który będzie poruszał odpowiednimi węzłami. Używamy do tego plugin *Rigify*, wbudowany w blender. Posiada od razu podstawowy szkielet człowieka. 
[Tu jest niezły poradnik](https://www.youtube.com/watch?v=m-Obo_nC3SM). Oglądamy do **7.31**.
[Tutaj jest jak poprawić automatycznie wygenerowany szkielet](https://www.youtube.com/watch?v=4fICQmBEt4Y). Trzeba będzie to zrobić m.in. w sytuacji, kiedy [[#4. Generowanie animacji|nasze wygenerowane animacje]] będą *dziwne*.
Warto spróbować ułożyć jakieś konkretne pozy, np. biegania żeby sprawdzić, czy poruszenie prawą ręką nie obróci lewej stopy itd.
![[rigged_axeman.png]]
[[#Nie mogę włączyć *Pose mode* w blender]]
[[#Nie mogę włączyć *Weight paint* w blender]]
[[#Nie mogę wybrać konkretnej kości w *Weight paint*]]
[[#Malowanie w trybie *Weight paint* nic nie zmienia]]
[[#Jak dodać przedmiot do zriggowanego modelu?]]
## 3. Export do FBX
Unity nie obsługuje formatów blenderowych. Cały nasz model z teksturami i szkieletem należy wyeksportować do **FBX**. To jest taki zip, który zawiera wszystko.
1. Na górze `File -> Export -> FBX`
2. Po prawej stronie zaznaczamy `Path Mode: Copy` oraz `Embedded textures`
![[export_fbx_1.png]]
![[export_fbx_2.png]]
[[#Mój model nie ma tekstur w Mixamo (jest szary)]]
## 4. Generowanie animacji
### Mixamo
https://www.mixamo.com/ - tutaj dla naszego **FBX** można automatycznie wygenerować animacje. Mixamo ma swój własny rigger, ale jest beznadziejny. Poza tym my już szkielet i tak mamy gotowy.

Animacji do wyboru jest dużo, są dosyć typowe np. poruszanie się postaci, atakowanie wręcz itd.
Potrzebne na pewno będą będą:
- Poruszanie się WASD
- Atak
- Śmierć
Na górze jest wyszukiwarka.
Po prawej stronie można dostosować parametry np. 
Pobieramy każdą animację jako osobny plik **FBX**. Później [[tworzenie_klasy_postaci#5. Import zanimowanego modelu do Unity|będziemy wyjmować z niego tylko animację]]. Co do ruchu lewo/prawo to polecam animacje **strafe left / right**.
Pobieramy 60fps, with skin (chociaż to akurat chyba nie ma znaczenia).

PS. zawsze robiłem `Format: FBX Binary`. Jest też `FBX for Unity`, nigdy tego nie testowałem. Być może oszczędzi to problemów w [[#5. Import zanimowanego modelu do Unity]].
![[mixamo.png]]
[[#Mój model w mixamo ma dziwne animacje]]
### Ręcznie
Animacje nietypowe trzeba zrobić samodzielnie.
Todo
## 5. Import zanimowanego modelu do Unity
# FAQ
### Nie mogę włączyć *Pose mode* w blender
Trzeba najpierw wejść w *Object mode*, wybrać szkielet, dopiero teraz *Pose mode* jest widoczny.
![[pose_mode.png]]
### Nie mogę włączyć *Weight paint* w blender
Trzeba najpierw wejść w *Object mode*, wybrać model **(nie szkielet)**, dopiero teraz *Weight paint* jest widoczny.
![[weight_paint.png]]
### Nie mogę wybrać konkretnej kości w *Weight paint*
Spróbuj wcisnąć `Alt`.

https://blender.stackexchange.com/questions/148726/cant-select-bones-in-weight-paint-mode

https://www.youtube.com/watch?v=WVp0asJsYfw
### Malowanie w trybie *Weight paint* nic nie zmienia
Dodaj więcej węzłów. Wagi w blenderze wyglądają jak ścianki albo strefy, ale tak naprawdę to wagi są określane na węzłach. 
Nie masz węzłów pomiędzy łokciem a barkiem? Nie zmienisz wagi na tym odcinku. Trzeba dorobić węzły pomiędzy. Np. `Ctrl+R` w *Edit mode*.
### Jak dodać przedmiot do zriggowanego modelu?
https://www.youtube.com/watch?v=3jCE2Va0ChM
### Mój model nie ma tekstur w Mixamo (jest szary)
Wyłącz i włącz komputer (pamiętaj, żeby zapisać swój plik blendera!)

Wyeksportuj w trybie `Path Mode: Copy` oraz `Embedded textures`
### Mój model w mixamo ma dziwne animacje
Popraw *Weight paint*. Dodaj więcej węzłów. Prawdopodobnie kości mają wpływ na węzły totalnie niezwiązane z tą kością, ponieważ były blisko podczas generowania szkieletu. Należy po prostu usunąć ten wpływ.