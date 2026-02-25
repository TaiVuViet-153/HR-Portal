import { useEffect, useMemo, useState } from 'react';
import Select from 'react-select';

type Option = { value: string; label: string };

type Props = {
  name: string;
  options: Option[];
  defaultValue?: string | number;
  placeholder?: string;
  isClearable?: boolean;
  isDisabled?: boolean;
  isRequired?: boolean;
  onChange?: (value?: string) => void;
};

export default function SelectCommon({ name, options, defaultValue, placeholder, isClearable, isDisabled, onChange }: Props) {
  const toOption = (val?: string | number) =>
    options.find(o => o.value === String(val)) ?? null;

  const [selected, setSelected] = useState<Option | null>(toOption(defaultValue));

  useEffect(() => {
    setSelected(toOption(defaultValue));
  }, [defaultValue, options]);

  const hiddenValue = useMemo(() => selected?.value ?? '', [selected]);

  return (
    <div className="relative">
      <Select
        value={selected}
        options={options}
        isClearable={isClearable}
        isDisabled={isDisabled}
        placeholder={placeholder ?? 'Select...'}
        onChange={(opt) => {
          const next = opt as Option | null;
          setSelected(next);
          onChange?.(next?.value);
        }}
        classNames={{
          control: (state) =>
            `!min-h-[44px] !rounded-xl !border !border-gray-200 !shadow-sm 
            ${state.isFocused ? '!ring-2 !ring-blue-500 !border-transparent' : ''}
            ${state.isDisabled ? '!bg-gray-100 !cursor-not-allowed' : ''}`,
          placeholder: () => '!text-gray-400',
          singleValue: () => '!text-gray-700 !font-medium',
          menu: () => '!rounded-xl !shadow-lg !border !border-gray-100',
          option: (state) =>
            `!text-sm !py-2 ${state.isFocused ? '!bg-blue-50 !text-blue-600' : '!text-gray-700'}`,
        }}
      />
      <input type="hidden" name={name} value={hiddenValue} />
    </div>
  );
};
