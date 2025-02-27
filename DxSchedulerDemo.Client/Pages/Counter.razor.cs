using DevExpress.Blazor;
using DxSchedulerDemo.Client.Data;
using DxSchedulerDemo.Client.Utils;
using Microsoft.AspNetCore.Components;

namespace DxSchedulerDemo.Client.Pages
{
    public partial class Counter : ComponentBase
    {
        int apptCount = 0;
        DateTime currentDate = DateTime.Today;
        DateTime startDate = new DateTime();
        DateTime endDate = new DateTime();
        DxScheduler? Scheduler;
        SchedulerViewType activeType = SchedulerViewType.Day;
        IEnumerable<Appointment> DataSource = new List<Appointment>();
        DxSchedulerDataStorage DataStorage = new DxSchedulerDataStorage()
        {
            AppointmentMappings = new DxSchedulerAppointmentMappings()
            {
                Id = "AppointmentId",
                Type = "AppointmentType",
                Start = "StartDate",
                End = "EndDate",
                Subject = "Caption",
                AllDay = "AllDay",
                Location = "Location",
                Description = "Description",
                LabelId = "Label",
                StatusId = "Status"
            }
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                LoadData();
                await ScrollToFirstAppointment();
            }
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadAppointments();
        }
        void OnStartDateChanged(DateTime newStartDate)
        {
            currentDate = newStartDate;
            LoadAppointments();
        }

        async Task OnActiveViewChanged(SchedulerViewType newView)
        {
            activeType = newView;
            LoadAppointments();
            await ScrollToFirstAppointment();
        }
        void LoadAppointments()
        {
            switch (activeType)
            {
                case SchedulerViewType.Day:
                    startDate = currentDate;
                    endDate = currentDate.AddDays(1);
                    break;
                case SchedulerViewType.Week:
                    startDate = currentDate.StartOfWeek(DayOfWeek.Sunday);
                    endDate = startDate.AddDays(7);
                    break;
                case SchedulerViewType.Month:
                    startDate = currentDate.StartOfMonth();
                    endDate = currentDate.AddMonths(1);
                    break;
            }
            LoadData();
        }

        private void LoadData()
        {
            this.DataSource = AppointmentCollection.GetAppointments(startDate, endDate);

            DataStorage.AppointmentsSource = this.DataSource;
            DataStorage.RefreshData();
            apptCount = this.DataSource.ToList().Count;
        }

        Appointment? GetFirstAppointment()
        {
            return this.DataSource.OrderBy(ds => ds.StartDate).FirstOrDefault();
        }

        async Task ScrollToFirstAppointment()
        {
            var appointment = GetFirstAppointment();
            if (appointment == null || this.Scheduler == null)
            {
                return;
            }

            await Task.Run(async () =>
            {
                await Task.Delay(150);
                await InvokeAsync(() => Scheduler.ScrollTo(appointment.StartDate));
            });
        }
    }
}
