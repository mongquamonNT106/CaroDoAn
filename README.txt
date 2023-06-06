-Game Caro có tích hợp thêm tính năng Chat trong game (cụ thể là 2 người chơi có thể Chat cho nhau)
-Game có 2 hình thức chơi là chơi qua mạng LAN (2 máy khác nhau) hoặc chơi cùng 1 máy (2 người chơi trên cùng 1 máy), mặc định khi bật Game lên, chế độ mặc định sẽ luôn là đang ở trạng thái 1 máy
-Game có các tính năng bao gồm: LAN, 1 máy, Undo, Redo
-LAN: yêu cầu cần 2 máy, máy đầu tiên ấn vào nút LAN sẽ tự động tạo phòng và sẽ chờ đợi người chơi thứ 2 vào, máy thứ 2 khi ấn vào nút LAN sẽ tự động được kết nối tới phòng của máy 1. Khi chưa ai vào phòng mà ta đánh trước, sẽ có thông báo lỗi là vẫn chưa có người chơi thứ 2 vào phòng
*Lưu ý: Chương trình hiện tại chỉ có thể host 1 room
-Nút Undo/Redo: xóa đi 2 nút mà 2 player vừa đánh gần đây nhất (player 1 sẽ bị undo 1 nút và player 2 sẽ bị undo 1 nút), ngược lại, redo sẽ trả lại 2 nút vừa bị undo gần nhất 
*Lưu ý: Undo và Redo chỉ có thể sử dụng 1 lần mỗi lượt, ví dụ như Player 1 xài Undo, thì khi tới lượt Player 2 sẽ không thể xài Undo, và khi quay lại lượt của Player 1 thì mới xài lại được Undo, tương tự Redo cũng vậy
-Nút Quit: Thoát game, nhưng nếu đang ở chế độ chơi LAN thì sẽ thông báo cho người chơi còn lại trong phòng biết là đã có người thoát khỏi phòng
-Có 2 hình thức thắng: thắng nếu đạt 5 nút thẳng hàng hoặc khi đối thủ hết thời gian, dựa vào loại hình thức thắng thì sẽ hiện thông báo thắng khác nhau.
-Khung Chat: Giúp cho 2 player có thể giao tiếp với nhau khi ở chế độ LAN.
-Nếu như chương trình thông báo thiếu file Resources thì có thể copy file Resources ở bên ngoài file Bin, hoặc tải trên Drive của nhóm:  https://drive.google.com/drive/folders/1kWhB6aI3yeMQbJbBk7-e897quTuU_1uc?fbclid=IwAR3fE0JvnGYt6wQxSHC4xX6TtMK0bjHfTCNUJSh6iLT0Tk3hw7vxCQnZ-tM