# LuckyFarmingCardGame Dev Log

## Day 0: 05.06.2023

**Time Track:** 1H

**Work done:**
- Trong lúc tìm một vài tài liệu trong Cloud cá nhân, tôi tìm thấy folder design về một project boardgame tôi từng làm khi còn là một Fresher Boardgame Designer

![Folder thiết kế sơ khai](https://github.com/DangHoang2109/LuckyFarmingCardGame/assets/32613745/8e98c314-ca4b-409b-a338-e7b6d02996c2)

- Sau khi đọc lại, tôi cảm thấy luật chơi này tuy không đủ hấp dẫn và chi tiết để phát triển thành một sản phẩm Boardgame để phát hành ra thị trường, đặc biệt là tại quốc gia Việt Nam của tôi. Tuy nhiên, nó vẫn có một vài điểm sáng trong game mechanic như push-your-luck, dice rolling có vẻ tiềm năng nếu được phát triển thành một dạng Mobile Game Casual
- Vì vậy, tôi bắt tay vào thực thi dự án này như một Indie game Developer, bao gồm chỉnh sửa update lại luật chơi, implement thành một sản phẩm qua từng giai đoạn, thiết kế hệ thống currency toàn game và mô hình kiếm tiền. 


## Day 1: 06.06.2023

**Time Track:** 2H30M

**Work done:**
- Tôi đã set up project trống trên Unity, tại phiên bản Unity 2022.3.0 LTS
![Project Home Scene Sketching](https://github.com/DangHoang2109/LuckyFarmingCardGame/assets/32613745/14b41152-2582-4d2e-833c-9f2665293b8a)

- Hoàn thành game core loop trong một turn của một user:
  - Rút một lá bài và đặt vào pallet
  - Xử lý luật chơi khi pallet trùng card: Theo luật chơi lúc này thì pallet sẽ conflict nếu có 2 card cùng loại trong pallet và sẽ yêu cầu user phải roll một dice
  - Xử lý luật chơi khi roll dice: Theo luật chơi lúc này thì tùy vào kết quả roll dice mà user sẽ giữ được card trong pallet về túi hoặc bị hủy tất cả
  - Xây dựng prefab card UI: Góc dưới phải là kí hiệu chức năng của card, góc dưới trái là số tiền nhận được khi thu hoạch, ảnh giữa card là ảnh của loại thẻ
  
  ![Card UI](https://github.com/DangHoang2109/LuckyFarmingCardGame/assets/32613745/77aa35a1-0ef6-4d5f-97c2-4a1f931ef6d6)
  
  - Luật choi được test trên Debug Console để giảm thiểu thời gian lãng phí cho việc set up UI vốn chưa được sketch

  ![Test Debug](https://github.com/DangHoang2109/LuckyFarmingCardGame/assets/32613745/06e4f87f-1767-4531-b2ce-cfb7a202e04d)
  
  
## Day 2: 07.06.2023

**Time Track:** 1H30M

**Work done:**
- Hoàn thành core loop trong toàn game:
  - Random một lượng player và cho join game
  - Implement turn base game, khi một user kết thúc lươt chủ động hoặc do pallet thì sẽ next turn qua user kế bên và xoay vòng
  - Kiểm tra kết thúc game bằng compare với card mission goal
- Implement card mission goal:
  - Add code và cho user model check để kiểm tra end game
  - Nhập liệu 9 card temp theo doc cũ
  
![image](https://github.com/DangHoang2109/LuckyFarmingCardGame/assets/32613745/0a6a1e0d-0579-4fc8-9c0d-bae6c27915d9)

## Day 3: 08.06.2023
**Time Track:** 2H

**Work done:**
- Hoàn thành hệ thống card effect, dễ mở rộng, đơn giản, hiện có 5 card effect trong proto ban đầu
- Hoàn thành hệ thống deck, có thể shuffle khi hết deck
![image](https://github.com/DangHoang2109/LuckyFarmingCardGame/assets/32613745/bc839f8e-6d92-403f-aeb6-8b6161980232)

## Day 4: 10.06.2023
**Time Track:** 5H

**Work done:**
- Dựng card item, spawn card và destroy card
- Hệ thống animation có thời gian cơ bản: 
  -  Animation rolling dice
  -  Animation destroy palelt
  -  Animation Pulling pallet
  -  Animation Conflict pallet
-  Thêm ảnh icon effect

Viewing the log track video at [D4.2023-06-10](https://drive.google.com/drive/folders/1kEiFyYvTiatmA-t5n3pk2L3kSWNORuYo?usp=sharing)


## Day 5: 11.06.2023
**Time Track:** 4H

**Work done:**
- layout user panel
- import avatar, avatar frame, basic sound
- cập nhật giá trị từ user bag vào UI
- Thêm UI biểu thị đang trong lượt
- animation effect reveal card
- Chỉnh size card trong pallet nhỏ lại còn 0.5f
- thêm toggle cho user bag UI để hỗ trợ cho effect có chọn card
- destroying and pulling card effect
- control deck ko cho draw thêm khi effect đang chạy
- update draw card effect thành chỉ còn draw 1 card thay vì 2

## Day 6: 12.06.2023
**Time Track:** 1H

**Work done:**
- thêm notification UI hướng dẫn khi có 1 card cần user tương tác kích hoạt
- thêm card effect description vào config

## Day 7: 14.06.2023
**Time Track:** 1H

**Work done:**
- set up dialog UI
- thêm instruction card effect quickthrough dialog
- set up show dialog code
- import coin point trong card module vào gameplay

Viewing the log track video at [D4.2023-06-11](https://drive.google.com/drive/folders/1kEiFyYvTiatmA-t5n3pk2L3kSWNORuYo?usp=sharing)

## Day 8: 16.06.2023
**Time Track:** 2H

**Work done:**
- thêm module dùng game coin để tăng giá trị dice roll ra nhằm tăng tính chiến thuật và giảm độ rủi ro
- Up lên Unity version 2022.3.2f1, là LTS mới nhất của unity 2022

## Day 9: 17.06.2023
**Time Track:** 2H

**Work done:**
- Bắt đầu làm AI cho Bot part 1


## Day 10: 18.06.2023
**Time Track:** 2H

**Work done:**
- Bắt đầu làm AI cho Bot part 2
  
## Day 11: 19.06.2023
**Time Track:** 2H

**Work done:**
- Hoàn thiện AI
- Hoàn thiện control phía Main control: bật tắt button end game, bật tắt button draw theo bot hay main
