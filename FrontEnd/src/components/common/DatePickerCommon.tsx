import { useCallback, useEffect, useMemo, useState } from 'react';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';

type Props = {
  name: string;
  defaultValue?: string;
  className?: string;
  disablePast?: boolean;
  minDate?: string;
  maxDate?: string;
  views?: Array<'year' | 'month' | 'day'>;
  displayFormat?: string;
};

export default function DatePickerCommon({ name, defaultValue, className, disablePast = true, minDate, maxDate, views = ['day'], displayFormat }: Props) {
  const [value, setValue] = useState<Dayjs | null>(defaultValue ? dayjs(defaultValue) : null);
  // const [holidays, setHolidays] = useState<string[]>([]);

  useEffect(() => {
    setValue(defaultValue ? dayjs(defaultValue) : null);
  }, [defaultValue]);

  useEffect(() => {
    // Call API to fetch holidays and set them in state
    // axios.get('/api/holidays').then(res => {
    //   setHolidays(res.data);
    // });
  }, []);

  const format = displayFormat ?? (views.length === 1 && views[0] === 'year' ? 'YYYY' : 'YYYY-MM-DD');
  const formatted = useMemo(() => (value ? value.format(format) : ''), [value, format]);

  // const holidaySet = useMemo(() => {
  //   return new Set(holidays);
  // }, [holidays]);

  const shouldDisableDate = useCallback((date: Dayjs) => {
    const dayOfWeek = date.day(); //

    // disable Saturday and Sunday, 0: Sunday, 6: Saturday
    return dayOfWeek === 0 || dayOfWeek === 6;

    // disable holiday
    // const key = date.format('YYYY-MM-DD');
    //   return holidaySet.has(key);

    // }, [holidaySet]);
  }, []);

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <DatePicker
        value={value}
        onChange={(newValue) => setValue(newValue)}
        views={views}
        format={format}
        disablePast={disablePast}
        minDate={minDate ? dayjs(minDate) : undefined}
        maxDate={maxDate ? dayjs(maxDate) : undefined}
        shouldDisableDate={views.includes('day') ? shouldDisableDate : undefined}
        slotProps={{
          textField: {
            fullWidth: true,
            placeholder: views.length === 1 && views[0] === 'year' ? 'Select year' : 'Select date',
            className:
              className ??
              'w-full px-4 py-3 bg-white border border-gray-200 rounded-xl shadow-[0_1px_2px_rgba(0,0,0,0.04)] focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 placeholder:text-gray-400'
          }
        }}
      />
      <input type="hidden" name={name} value={formatted} />
    </LocalizationProvider>
  );
};
