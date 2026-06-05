import { useEffect, useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Button, Tag, Space, Form, Input,
  InputNumber, Switch, App, Tooltip, Checkbox,
} from 'antd'
import AppModal from '@/components/AppModal'
import AgGridWrapper from '@/components/AgGridWrapper'
import { PlusOutlined, EditOutlined, StopOutlined, CheckOutlined, DeleteOutlined } from '@ant-design/icons'
import type { ColDef, ICellRendererParams } from 'ag-grid-community'
import { countryApi, type CountryResponse, type CreateCountryRequest } from '@/api/codebook'
import CodebookLayout from '@/layouts/CodebookLayout'

type FormValues = Omit<CountryResponse, 'id'>

export default function CountryPage() {
  const { t } = useTranslation()
  const { message, modal } = App.useApp()

  const [data, setData]               = useState<CountryResponse[]>([])
  const [loading, setLoading]         = useState(false)
  const [onlyActive, setOnlyActive]   = useState(true)
  const [modalOpen, setModalOpen]     = useState(false)
  const [saving, setSaving]           = useState(false)
  const [editingItem, setEditingItem] = useState<CountryResponse | null>(null)
  const [form] = Form.useForm<FormValues>()

  const fetchData = async () => {
    setLoading(true)
    try {
      const res = await countryApi.getAll(true)
      setData(res.data)
    } catch {
      message.error('Greška pri dohvatu podataka')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchData() }, [])

  const filteredData = onlyActive ? data.filter(d => d.isActive) : data

  const openAddModal = () => {
    setEditingItem(null)
    form.resetFields()
    form.setFieldsValue({ isActive: true, ordinal: data.length + 1 })
    setModalOpen(true)
  }

  const openEditModal = (item: CountryResponse) => {
    setEditingItem(item)
    form.setFieldsValue(item)
    setModalOpen(true)
  }

  const handleSave = async () => {
    const values = await form.validateFields()
    setSaving(true)
    try {
      if (editingItem) {
        await countryApi.update(editingItem.id, values)
        message.success('Zapis ažuriran')
      } else {
        await countryApi.create(values as CreateCountryRequest)
        message.success('Zapis dodan')
      }
      setModalOpen(false)
      fetchData()
    } catch {
      message.error('Greška pri spremanju')
    } finally {
      setSaving(false)
    }
  }

  const handleToggleActive = async (item: CountryResponse) => {
    try {
      await countryApi.toggleActive(item.id)
      message.success(item.isActive ? 'Zapis deaktiviran' : 'Zapis aktiviran')
      fetchData()
    } catch {
      message.error('Greška pri promjeni statusa')
    }
  }

  const handleDelete = async (item: CountryResponse) => {
    try {
      await countryApi.deleteById(item.id)
      message.success('Zapis obrisan')
      fetchData()
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status
      message.error(status === 409 ? t('common.error_in_use') : t('common.error_delete'))
    }
  }

  const confirmToggleActive = (item: CountryResponse) => {
    modal.confirm({
      title: t(item.isActive ? 'codebook.country.confirm.deactivate' : 'codebook.country.confirm.activate'),
      onOk: () => handleToggleActive(item),
      okText: t('common.yes'),
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const confirmDelete = (item: CountryResponse) => {
    modal.confirm({
      title: t('codebook.country.confirm.delete'),
      content: t('codebook.country.confirm.delete_description'),
      onOk: () => handleDelete(item),
      okText: t('codebook.country.confirm.delete_ok'),
      okType: 'danger',
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const columnDefs = useMemo<ColDef<CountryResponse>[]>(() => [
    {
      field: 'code',
      headerName: t('codebook.country.columns.code'),
      width: 80,
    },
    {
      field: 'nameHr',
      headerName: t('codebook.country.columns.nameHr'),
      flex: 1,
    },
    {
      field: 'nameEn',
      headerName: t('codebook.country.columns.nameEn'),
      flex: 1,
      valueFormatter: (p) => p.value ?? '—',
    },
    {
      field: 'ordinal',
      headerName: t('codebook.country.columns.ordinal'),
      width: 120,
      sort: 'asc',
    },
    {
      field: 'isActive',
      headerName: t('codebook.country.columns.status'),
      width: 120,
      filter: false,
      cellRenderer: (p: ICellRendererParams<CountryResponse>) =>
        p.value
          ? <Tag color="success">{t('codebook.country.status.active')}</Tag>
          : <Tag color="default">{t('codebook.country.status.inactive')}</Tag>,
    },
    {
      headerName: t('codebook.country.columns.actions'),
      width: 120,
      sortable: false,
      filter: false,
      resizable: false,
      cellRenderer: (p: ICellRendererParams<CountryResponse>) => {
        const rec = p.data!
        return (
          <Space size={4}>
            <Tooltip title={t('codebook.country.actions.edit')}>
              <Button type="text" icon={<EditOutlined />} size="small" onClick={() => openEditModal(rec)} />
            </Tooltip>
            <Tooltip title={t(rec.isActive ? 'codebook.country.actions.deactivate' : 'codebook.country.actions.activate')}>
              <Button type="text" icon={rec.isActive ? <StopOutlined /> : <CheckOutlined />}
                size="small" danger={rec.isActive} onClick={() => confirmToggleActive(rec)} />
            </Tooltip>
            <Tooltip title={t('codebook.country.actions.delete')}>
              <Button type="text" icon={<DeleteOutlined />} size="small" danger onClick={() => confirmDelete(rec)} />
            </Tooltip>
          </Space>
        )
      },
    },
  ], [t, openEditModal, confirmToggleActive, confirmDelete])

  return (
    <CodebookLayout
      title={t('codebook.country.title')}
      extra={
        <Space>
          <Checkbox checked={onlyActive} onChange={e => setOnlyActive(e.target.checked)}>
            {t('common.only_active')}
          </Checkbox>
          <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal}>
            {t('codebook.country.addNew')}
          </Button>
        </Space>
      }
    >
      <AgGridWrapper<CountryResponse>
        columnDefs={columnDefs}
        rowData={filteredData}
        loading={loading}
        getRowId={(p) => p.data.id}
        getRowStyle={(p) => p.data?.isActive ? undefined : { opacity: 0.45 }}
      />

      <AppModal
        title={editingItem ? t('codebook.country.modal.editTitle') : t('codebook.country.modal.addTitle')}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        okText={t('common.save')}
        cancelText={t('common.cancel')}
        confirmLoading={saving}
        destroyOnClose
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item
            name="code"
            label={t('codebook.country.modal.code')}
            tooltip={t('codebook.country.modal.code_hint')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={10} style={{ textTransform: 'uppercase' }} />
          </Form.Item>
          <Form.Item
            name="nameHr"
            label={t('codebook.country.modal.nameHr')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="nameEn" label={t('codebook.country.modal.nameEn')}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="ordinal" label={t('codebook.country.modal.ordinal')}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="isActive" label={t('codebook.country.modal.isActive')} valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </AppModal>
    </CodebookLayout>
  )
}
