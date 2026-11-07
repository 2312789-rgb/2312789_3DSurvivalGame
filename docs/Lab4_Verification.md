# Lab 4 - Verification

Project: `2312789_3DSurvivalGame` - MSSV `2312789`  
Unity: `6000.5.6f1` - Scene: `Assets/Scenes/SampleScene.unity`  
Date: 2026-11-06 (Asia/Saigon)

## 1. Phạm vi và tài liệu

Tiếp tục dự án Lab 3, bổ sung UI Crafting và mã điều khiển Inventory/Crafting. Không tạo dự án mới, không truy cập hay sửa TreesHateYou, không commit/push Git.

Đã đọc toàn bộ 2 trang `C:/Users/PC/Downloads/Lab 04 - Crafting System.pdf` và kiểm tra ảnh render hai trang. PDF yêu cầu UI nâng cao, lấy vật phẩm vào Inventory, tăng/giảm số lượng, tiếp tục Lab 3, thực hiện hai video, xuất Windows exe và nộp `2312789_Lab4_exe.rar`, `Lab 4.rar`. PDF không mô tả chi tiết thuật toán Crafting hoặc xác nhận công thức 3+3; các chi tiết này lấy từ yêu cầu người dùng.

Video được PDF và người dùng chỉ định:

- Part 7: https://www.youtube.com/watch?v=fxyt35_Bh-M
- Part 8: https://www.youtube.com/watch?v=JJXNVS_-tGI

Giới hạn đối chiếu: công cụ web không tải được hai trang và oEmbed; tìm kiếm theo ID không trả nội dung, truy cập Python gặp lỗi phân giải DNS. Có quan sát được ảnh trình duyệt của video Part 7 đang mở ở đoạn cuối, hiển thị tên Crafting System (UI), Tools và Craft logic; **không xem toàn bộ video**, không lấy được mô tả/transcript, không xác minh được các liên kết tài nguyên trong mô tả. Vì vậy độ khớp từng chi tiết với video là **PARTIAL**, không tuyên bố hoàn thành đối chiếu video 100%.

## 2. Phân tích trước triển khai

Đã liệt kê toàn bộ tệp và thư mục Assets trong `Assets_Inventory.txt`, `Assets_Directories.txt`, đọc các script trong `Assets/Scripts`, kiểm tra Scene qua Unity MCP. Canvas có InventoryScreen và 21 ItemSlot; EventSystem dùng StandaloneInputModule; Camera nằm dưới Player. Project bật `activeInputHandler: 2` (Both), nên giữ legacy Input đã có, không thêm Input Actions hoặc module UI cạnh tranh.

| Chức năng | Đã có | Còn thiếu | Cần sửa | Cách triển khai |
|---|---|---|---|---|
| Player, Camera, Terrain, House, Rabbit, Stone | Có | Không thuộc Lab 4 | Camera/cursor khi UI mở | Giữ GameObject, dữ liệu và chuyển động gốc; thêm điều kiện UI |
| Inventory | 21 ô, prefab stone, phím I | Đếm vật phẩm, trừ nguyên liệu, giao dịch | Chỉ nhặt khi thêm thành công; đặt item giữa ô | Mở rộng InventorySystem hiện có |
| Kéo thả | DragDrop, ItemSlot | Không | Null guard, Canvas scaling | Giữ cơ chế cũ, cập nhật delta theo scaleFactor |
| Chọn/nhặt | Raycast và Interactable | Một nguồn lựa chọn thống nhất | Tia con trỏ khác tia giữa màn hình; nguy cơ xóa khi không thêm được | SelectionManager chọn duy nhất, kiểm tra lại raycast trong TryPickup |
| Crafting UI | Chưa có | Screen, Tools, Blueprint, Back, Craft | Toàn bộ | Lưu UI trong Canvas hiện có và persistent button events |
| Công thức/dữ liệu | Chưa có | 3 stone + 3 Stick -> 1 StoneAxe | Toàn bộ | Blueprint ScriptableObject, Requirement, RecipeView |
| Build/nộp | Windows module đã cài | Profile và bản Windows Lab 4 | Tạo riêng, không ghi đè build cũ | Windows Build Profile chỉ chứa SampleScene |

Backup trước sửa: `Backups/Lab4_20261106_141907/` chứa Scripts, Scenes, Resources, Packages và ProjectSettings. Các GUID script cũ được giữ bằng việc không thay file .meta. Không xóa asset hay GameObject gốc. CanvasScaler được đổi sang Scale With Screen Size 1600x900 để UI mới co giãn.

## 3. Tài nguyên sử dụng

- Terrain, Player, Camera, Rabbit, House, hai Stone gốc và giao diện Inventory: tái sử dụng từ dự án hiện tại, không tải gói mới. Không xác minh được URL gốc của từng asset nên không gán URL suy đoán.
- `Assets/Resources/stone.prefab` và `stone.png`: dùng lại; prefab cũ không bị sửa.
- Stick và StoneAxe: icon 96x96 tự tạo bằng mã trong Lab4Setup, lưu PNG/Sprite và UI prefab. Không lấy mã nguồn/icon từ video.
- Sáu vật phẩm thế giới mới: 3 Sphere dạng đá và 3 Capsule dạng cành cây, dùng primitive của Unity và material tự tạo. Đây là vật liệu đại diện thay model chưa xác minh từ video.
- Stone Axe là **vật phẩm Inventory ảo**; không thêm cơ chế chặt cây, trang bị vũ khí, damage hoặc các nội dung video sau.

## 4. Script thêm/sửa và tích hợp Scene

Thêm:

- `Assets/Scripts/Blueprint.cs`: dữ liệu kết quả và danh sách nguyên liệu.
- `Assets/Scripts/CraftingSystem.cs`: mở/đóng UI, danh mục, yêu cầu Craft, cập nhật HUD.
- `Assets/Scripts/CraftingRecipeView.cs`: tên, icon, số lượng và trạng thái nút.
- `Assets/Scripts/InventoryItem.cs`: định danh item thực tế trong ô chứa.
- `Assets/Scripts/GameplayUI.cs`: trạng thái UI, khóa/hiện chuột và dấu ngắm.
- `Assets/Editor/Lab4Setup.cs`: công cụ tích hợp một lần, có kiểm tra đúng project/Scene, từ chối ghi đè khi đã cài. Chỉ chạy khi được gọi rõ ràng; không tự chạy lúc mở project/build.

Sửa: `InventorySystem.cs`, `SelectionManager.cs`, `InteractableObject.cs`, `MouseMovement.cs`, `PlayerMovement.cs`, `DragDrop.cs`, `ItemSlot.cs`.

GameObject thêm:

- Root `CraftingSystem` (một manager duy nhất).
- `Canvas/CraftingScreen`: Title, Subtitle, Categories/ToolsButton, ToolsCategoryScreen/StoneAxeBlueprint, Icon, ItemName, Requirements, CraftButton, BackButton, CloseButton, Status.
- `Canvas/Lab4HUD`, `Canvas/AimDot`.
- Root `Lab4_Materials`: Stone Pickup 1..3, Stick Pickup 1..3, đặt gần vị trí khởi đầu của Player trên Terrain.

Blueprint lưu tại `Assets/Lab4/StoneAxeBlueprint.asset`. Prefab `Assets/Resources/Stick.prefab`, `StoneAxe.prefab`; icon/material trong `Assets/Lab4/`. References và onClick được lưu trong Scene bằng UnityEditor API gọi qua Unity MCP. Kiểm tra đã có persistent event cho Tools và Craft, không có Missing Script. Không tạo UI bằng mã chạy lúc game khởi động.

## 5. Cách hoạt động để vấn đáp

InventorySystem giữ danh sách InventoryItem thực tế, mỗi vật phẩm chiếm một ô. `Count(id)` đếm theo định danh, không phụ thuộc vị trí kéo thả. `Changed` báo RecipeView và HUD cập nhật. Dữ liệu hiện chỉ tồn tại trong phiên chơi, không bổ sung save/load ngoài phạm vi Lab 4.

Blueprint là ScriptableObject lưu tên, icon, prefab kết quả trong Resources và các Requirement. Muốn thêm công thức, tạo Blueprint mới, thêm thẻ RecipeView trong Tools, gán references và đưa vào mảng của CraftingSystem; không phải viết lại thuật toán Craft.

Quy trình Craft: gộp yêu cầu cùng ID -> kiểm tra số lượng -> kiểm tra số ô sau khi tiêu hao nguyên liệu -> kiểm tra prefab kết quả -> tạm tách nguyên liệu nhưng chưa hủy -> tạo đủ kết quả -> cập nhật danh sách -> hủy nguyên liệu đã dùng -> phát Changed. Nếu lỗi khi tạo kết quả, hủy phần kết quả vừa tạo và trả nguyên liệu về ô ban đầu. Có khóa giao dịch và thời gian chờ 0.2 giây trên UI; mỗi lần vẫn kiểm tra lại dữ liệu, tránh nhân đôi.

SelectionManager dùng một tia giữa Camera, bỏ qua trigger, giới hạn 5m. TryPickup chỉ nhận đúng Selected và raycast còn hợp lệ; chỉ ẩn/hủy vật phẩm sau TryAdd thành công. Không còn mỗi Interactable tự xử lý cùng click.

Phím: WASD di chuyển, chuột xoay, Space nhảy, LMB nhặt vật đang nhìn, I Inventory, C Crafting, Esc đóng UI. Mở một UI đóng UI còn lại; chuột mở khóa, dừng điều khiển góc nhìn và di chuyển ngang khi UI mở. Đóng UI khóa chuột lại. Không đổi trọng lực hay dữ liệu chuyển động cũ.

## 6. Kiểm thử đã thực hiện

Các tệp `Lab4_RuntimeTest_1.txt`, `Lab4_RuntimeTest_2.txt`, `Lab4_PickupTest.txt`, `Lab4_UIInputTest.txt` là kết quả trả về từ kiểm thử thực tế trong Play Mode qua Unity MCP và thao tác cửa sổ Unity, không phải dữ liệu giả hiển thị trong UI.

| Test | Trạng thái | Bằng chứng/phạm vi |
|---|---|---|
| Scene chạy và khởi tạo lại | PASS | Đã Stop/Play nhiều lần, 21 ô, Inventory mới rỗng, UI đóng |
| Player di chuyển | PASS | Phím W thực tế, tọa độ ngang thay đổi; chưa kiểm thử đủ mọi địa hình/nhảy |
| Nhặt Stone/Stick | PASS | Nhặt đủ 6 GameObject thật bằng SelectionManager + TryPickup, nhận 3/3 |
| Hai vật gần nhau | PASS | Vật không Selected bị chặn, Inventory không tăng |
| Ngoài tầm, bị che, nhặt lặp | PASS | Raycast 7m, vật cản thử nghiệm và gọi lần hai đều bị chặn |
| Mở Crafting bằng C | PASS | Gửi phím vào Game View, ảnh cửa sổ và trạng thái MCP |
| Tools, Back | PASS | Click chuột thật, kiểm tra màn hình đúng |
| Thiếu nguyên liệu | PASS | TryCraft=false, nút disabled |
| Đủ nguyên liệu | PASS | UI hiện stone 3/3, Stick 3/3 và nút bật |
| Craft bằng nút | PASS | Click thật: stone 0, Stick 0, StoneAxe 1 |
| Craft liên tục | PASS | Gọi onClick hai lần và kiểm tra chỉ có 1 kết quả; lần sau thiếu nguyên liệu |
| Inventory đầy | PASS | 21 item, không nhận thêm; công thức kết quả 7 item bị chặn không mất nguyên liệu |
| Craft khi đầy nhưng giải phóng đủ ô | PASS | 21 -> 16 item, nhận đúng một rìu |
| Prefab kết quả không tồn tại | PASS | Công thức thử bị chặn, Inventory không đổi |
| Rollback khi Instantiate thực sự ném exception | NOT TESTED | Đã có code rollback; chưa cố ý phá prefab/engine để ép lỗi giữa giao dịch |
| Inventory/Crafting loại trừ nhau | PASS | Phím I thực tế mở Inventory, đóng Crafting; số lượng đồng bộ |
| Camera không xoay khi thao tác UI | PASS | Quaternion trước/sau click Craft giống nhau |
| Đóng UI khôi phục chuột | PASS | I đóng UI; Cursor Locked, visible=false |
| Đóng/mở lại không đổi dữ liệu | PASS | StoneAxe vẫn là 1 |
| Kéo thả qua EventSystem | PASS | Gọi beginDrag/drop/endDrag; vật đổi ô, số lượng giữ nguyên, drag cleared |
| Kéo thả bằng chuột vật lý | NOT TESTED | Thao tác tự động không di chuyển được item; không ghi PASS. Có lần bị gián đoạn bởi input/Editor reload |
| References/manager/missing script | PASS | Kiểm tra Scene: 0 Missing Script, manager riêng duy nhất, các button event đã lưu |
| Co giãn nhiều độ phân giải | PARTIAL | UI đã dùng anchors/scaler, đã quan sát Game View hiện tại; chưa kiểm đủ mọi tỷ lệ màn hình |

Ảnh minh chứng trong `docs/Screenshots/`: Crafting_Ready.png, Inventory_Result.png. Ảnh Crafting_Tools.png là lần QA đầu, trước khi sửa import icon; giữ lại làm bằng chứng phát hiện lỗi, không dùng làm ảnh giao diện cuối.

Kiểm thử thủ công bổ sung: Play, nhìn xuống các nguyên liệu gần điểm bắt đầu, LMB nhặt; C -> Tools -> Craft; I xem rìu, kéo qua ô trống và thả ngoài Inventory để kiểm tra trả về; C/I/Esc chuyển UI; thử ở 1280x720, 1920x1080, 4:3 và ultrawide. Không dùng số lượng cấp bằng test làm bằng chứng cho đường nhặt đồ; đường nhặt đã được kiểm riêng với 6 vật thật.

## 7. Console và hạn chế hiện có

Các lần đọc Console Error sau compile và sau chạy kiểm thử trả 0. Có một truy vấn chẩn đoán MCP gặp NullReference khi Editor vừa reload giữa phiên kiểm thử; đó là mã truy vấn tạm, không nằm trong script game. Đã dừng/chạy lại và tiếp tục kiểm thử thành công.

Cảnh báo thấy trong Editor: Unity AI Assistant Account API timeout 30 giây; `TerrainCollider: MeshCollider is not supported on terrain at the moment`. Không thay Terrain hoặc các gói AI để xử lý ngoài phạm vi. Rabbit và tree không có UI prefab tương ứng trong Resources từ Lab 3: TryAdd từ chối an toàn và giữ nguyên vật thể; không thêm chức năng thu hoạch Rabbit/tree.

## 8. Build và đóng gói

Windows Build Profile: `Assets/Settings/Build Profiles/2312789 Lab4 Windows.asset`, active, override Scene list chỉ có SampleScene.

Trạng thái build: **PASS / Succeeded**, BuildReport trả **0 errors**, **490 warnings**, 170,970,388 bytes (163.05 MiB), 920.18 giây. Chi tiết: `Lab4_BuildResult.json`, `Lab4_BuildWarnings.json`. Cảnh báo có shader của Unity AI Inference/Sentis (signed/unsigned, integer modulus, biến vòng lặp) và các shader/package sẵn có; không xóa package hoặc đổi renderer ngoài phạm vi để che cảnh báo.

Đã thử khởi chạy exe bằng Start-Process (PID trả về 6660), nhưng chưa thu được log runtime hoặc xác nhận cửa sổ. Người dùng đã dừng Computer Use bằng Escape; không tiếp tục thao tác UI. Vì vậy **chạy thử standalone: NOT TESTED / chưa xác minh**, không ghi PASS. Kiểm thử chức năng PASS ở trên là trong Unity Editor Play Mode.

Sau build, SampleScene được khôi phục đúng đường dẫn, không dirty. Editor hiện đang Play Mode tại thời điểm đọc BuildReport; không can thiệp phiên chơi đang mở sau khi người dùng dừng Computer Use.

Đường dẫn đầu ra:

- `Builds/Lab4_Windows/2312789_Lab4.exe` cùng `_Data` và các DLL/phụ thuộc.
- `Deliverables/2312789_Lab4_exe.rar`.
- `Deliverables/Lab 4.rar`, bao gồm Assets (và .meta), Packages, ProjectSettings, docs; loại Library, Temp, Logs, Obj, Backups và Builds.

WinRAR Rar.exe 7.12 có sẵn; sử dụng RAR thật, không đổi đuôi ZIP. Cả hai gói RAR đã qua kiểm tra CRC bằng rar t, exit 0. Xem ArchiveVerification.txt và SHA256SUMS.txt trong Deliverables để đối chiếu kết quả cuối. Thư mục BurstDebugInformation_DoNotShip không cần để chạy nên không đưa vào gói exe; mọi thư viện và thư mục dữ liệu chạy game được giữ.

## 9. Kết luận hiện tại

- Triển khai chức năng Lab 4 theo yêu cầu người dùng: PASS trong phạm vi kiểm thử nêu trên.
- Đối chiếu nguyên văn/từng bước hai video và liên kết mô tả: PARTIAL do hạn chế truy cập, không tải tài nguyên chưa rõ nguồn.
- Một số thử nghiệm bổ sung: NOT TESTED như bảng trên.
- Build Windows: **PASS**, 0 errors / 490 warnings; hai gói RAR được kiểm tra bằng Rar.exe trước khi bàn giao.
- Chạy thử exe: **NOT TESTED / chưa xác minh**, Computer Use bị người dùng dừng bằng Escape.
- Tổng thể: **PARTIAL** vì còn đối chiếu video đầy đủ và kiểm thử standalone, dù triển khai, Play Mode test và build đã hoàn tất.

Kiểm tra bảo toàn: `Lab4_PreservationCheck.json` xác nhận đủ 204 khối đối tượng Scene gốc, không thiếu khối cũ, tất cả Resources gốc giữ nguyên SHA-256, toàn bộ .meta script cũ giữ nguyên SHA-256. Đây là kiểm tra bảo toàn đối tượng/tài nguyên, không thay thế kiểm thử hành vi toàn bộ Lab 3.
