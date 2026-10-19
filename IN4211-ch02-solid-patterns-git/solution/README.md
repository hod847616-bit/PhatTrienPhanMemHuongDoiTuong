# EduHub Ch02 — Đáp án mẫu (dành cho giảng viên)

Refactor `ClubManager` (God class trong `starter/`) theo SOLID và 4 Design Pattern: Factory, Decorator, Strategy, Observer. Có unit test AAA cho lớp Domain.

## Chạy

```bash
cd 02_Projects/ch02/solution
dotnet build EduHub.Ch02.sln      # 0 lỗi, 0 cảnh báo
dotnet test EduHub.Ch02.sln       # 21 test xanh
dotnet run --project EduHub.Ch02  # demo 5 khối output
```

## Cấu trúc

| Thư mục | Nội dung | Nguyên tắc / Pattern |
|---|---|---|
| `Domain/` | `Club` (Create, HasRoom), `Member`, `Event` (Register, CheckIn, AttendanceRate) | quy tắc nghiệp vụ nằm ở Domain |
| `Repositories/` | `IClubRepository`, `IMemberRepository`, `IEventRepository`, bản `InMemory*`, `LoggingClubRepository` | Repository, **Decorator**, ISP |
| `Notifications/` | `INotificationService`, `Email/Sms/Slack`, `INotificationServiceFactory`, `NotificationServiceFactory` | **Factory**, OCP |
| `Patterns/Observers/` | `IEventObserver`, `EmailNotifier`, `LogNotifier` | **Observer** |
| `Patterns/Strategies/` | `IReportStrategy`, `ConsoleReportStrategy`, `CsvReportStrategy` | **Strategy**, OCP |
| `Services/` | `ClubService`, `EventService`, `ReportService` + interface | SRP, DIP |
| `Logging/` | `IAppLogger`, `ConsoleLogger`, `FileLogger` | DIP |
| `Program.cs` | Composition root: nơi duy nhất `new` class cụ thể | DIP |
| `EduHub.Ch02.Tests/` | xUnit: `ClubTests`, `EventTests` (Domain), `PatternTests` (4 pattern) | Arrange-Act-Assert |

## Vi phạm trong starter và cách sửa

- SRP: `CreateClub` vừa validate, lưu, log, thông báo thì tách thành `ClubService` + logger + notifier; luật tên CLB về `Club.Create`.
- OCP: if/else Gmail/Yahoo, Slack cố định thì dùng `INotificationService` + Factory; `PrintClubReport` thì dùng `IReportStrategy`.
- DIP: `File.AppendAllText`, `Console.WriteLine` trong logic thì dùng `IAppLogger`, `INotificationService`.
- ISP/DIP: list `_clubs`, `_members` giữ ngay trong class thì dùng repository interface nhỏ.
- Thông báo cứng khi tạo sự kiện thì dùng Observer (`EventService.Subscribe`).

## Demo trên lớp (gợi ý thứ tự)

1. `dotnet run`, chỉ ra 5 khối output (tạo CLB, Observer, Strategy, Factory, validate).
2. Khối 2: có 2 observer thì ra 1 dòng `[LOG]` và 2 dòng `[EMAIL]`; `EventService` không biết observer làm gì.
3. Khối 3: cùng dữ liệu, 2 định dạng (Console/CSV).
4. Dòng `[repo] ...` do Decorator `LoggingClubRepository`; bỏ decorator ở `Program.cs` thì dòng này biến mất, service không đổi.
5. Khối 4: thêm kênh mới = thêm class + một nhánh trong `NotificationServiceFactory`.
6. `dotnet test`: mở `EventTests` để chỉ cấu trúc AAA.

GUID trong output thay đổi mỗi lần chạy.
