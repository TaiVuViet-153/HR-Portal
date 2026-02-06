import React, { useEffect, useMemo, useState } from 'react';
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
};

export default function DatePickerCommon({ name, defaultValue, className, disablePast = true, minDate, maxDate }: Props) {
  const [value, setValue] = useState<Dayjs | null>(defaultValue ? dayjs(defaultValue) : null);

  useEffect(() => {
    setValue(defaultValue ? dayjs(defaultValue) : null);
  }, [defaultValue]);

  const formatted = useMemo(() => (value ? value.format('YYYY-MM-DD') : ''), [value]);

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <DatePicker
        value={value}
        onChange={(newValue) => setValue(newValue)}
        format="YYYY-MM-DD"
        disablePast={disablePast}
        minDate={minDate ? dayjs(minDate) : undefined}
        maxDate={maxDate ? dayjs(maxDate) : undefined}
        slotProps={{
          textField: {
            fullWidth: true,
            placeholder: 'Select date',
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
